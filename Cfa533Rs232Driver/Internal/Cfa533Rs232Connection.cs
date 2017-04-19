using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Petrsnd.Cfa533Rs232Driver.Internal
{
    internal class Cfa533Rs232Connection : IDisposable
    {
        private readonly string _serialPortName;
        private int _baudRate;
        private SerialPort _serialPort;
        private readonly Queue<byte> _readBuffer = new Queue<byte>(20);
        private readonly object _locker = new object();

        public Cfa533Rs232Connection(string serialPortName, int baudRate)
        {
            _serialPortName = serialPortName;
            _baudRate = baudRate;
        }

        public void Connect()
        {
            try
            {
                if (Connected)
                    Disconnect();

                _serialPort = new SerialPort(_serialPortName, _baudRate, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.RequestToSendXOnXOff,
                    ReadTimeout = 2000,
                    WriteTimeout = 2000,
                    ReadBufferSize = _baudRate,
                    WriteBufferSize = _baudRate,
                    Encoding = Encoding.ASCII
                };
                ConnectHandlers();
                _serialPort.Open();
                Log.Debug("Serial port connected");
            }
            catch (Exception ex)
            {
                throw new DeviceConnectionException("Unable to connect to LCD device", ex);
            }
        }

        public void Reconnect(int baudRate)
        {
            _baudRate = baudRate;
            Disconnect();
            Connect();
        }

        public void Disconnect()
        {
            if (_serialPort == null)
                return;
            _serialPort.Close();
            DisconnectHandlers();
            _serialPort.Dispose();
            _serialPort = null;
            Log.Debug("Serial port disconnected");
        }

        public bool Connected => _serialPort != null && _serialPort.IsOpen;

        public event EventHandler<KeypadActivityEventArgs> KeypadActivity; 

        public CommandPacket SendReceive(CommandPacket command)
        {
            ThrowIfNotConnected();
            try
            {
                // The Cfa533 documentation states that reconciling packets is the
                // best way to be sure that you don't have any dropped packets.
                // This is a limited device that cannot necessarily handle an
                // arbitrary number of packets without overflow. It is better to
                // acknowledge each response before continuing.
                Log.Debug("SEND: Waiting for lock");
                lock (_locker)
                {
                    var commandBuffer = command.ConvertToBuffer();
                    Log.Debug("SEND: {PacketType}:{CommandType} -- {ResponsePacketData}", command.PacketType,
                        command.CommandType, BitConverter.ToString(commandBuffer));
                    _serialPort.Write(commandBuffer, 0, commandBuffer.Length);
                    try
                    {
                        using (var t = WaitForResponseWithTimeout(command.CommandType, TimeSpan.FromSeconds(2)))
                        {
                            t.Wait();
                            if (t.Result == null)
                                throw new DeviceTimeoutException("Timeout while waiting for LCD device response");
                            return t.Result;
                        }
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.InnerException != null)
                            throw ex.InnerException;
                        throw ex.Flatten();
                    }
                }
                
            }
            catch (AggregateException ex)
            {
                throw new DeviceCommandException($"Command '{command.CommandType}' failed", ex.Flatten());
            }
        }

        private void ThrowIfNotConnected()
        {
            if (!Connected)
                throw new DeviceConnectionException("LCD device is not connected");
        }

        private void ConnectHandlers()
        {
            if (_serialPort == null)
                return;
            _serialPort.PinChanged += PinChangeHandler;
            _serialPort.DataReceived += DataReceivedHandler;
            _serialPort.ErrorReceived += ErrorReceivedHandler;
        }

        private void DisconnectHandlers()
        {
            if (_serialPort == null)
                return;
            _serialPort.PinChanged -= PinChangeHandler;
            _serialPort.DataReceived -= DataReceivedHandler;
            _serialPort.ErrorReceived -= ErrorReceivedHandler;
        }

        private static void PinChangeHandler(object sender, SerialPinChangedEventArgs e)
        {
            Log.Debug("Serial port pin change: {PinEvent}", e.EventType);
        }

        private event EventHandler<CommandPacketResponseReceivedEventArgs> ResponseReceived;

        private CommandPacket TryParsePacket()
        {
            try
            {
                var packet = CommandPacketParser.Parse(_readBuffer.ToArray());
                if (packet == null)
                {
                    Log.Debug("Null packet without error");
                    return null;
                }
                for (var i = 0; i < packet.PacketSizeWithCrc; i++)
                    _readBuffer.Dequeue();
                return packet;
            }
            catch (Exception ex)
            {
                Log.Debug("Parse response packet exception: {ParseError}", ex.Message);
                return null;
            }
        }

        private void ReadAllAvailableBytes()
        {
            Log.Debug("RECV: Reading available bytes");
            while (_serialPort.BytesToRead > 0)
            {
                var b = (byte)_serialPort.ReadByte();
                _readBuffer.Enqueue(b);
            }
            Log.Debug("RECV: Current read buffer: {ReadBuffer}", BitConverter.ToString(_readBuffer.ToArray()));
        }

        private void HandleResponsePacket(CommandPacket packet)
        {
            Log.Debug("RECV: {PacketType}:{CommandType} -- {ResponsePacketData}", packet.PacketType,
                    packet.CommandType, BitConverter.ToString(packet.ConvertToBuffer()));
            switch (packet.PacketType)
            {
                case PacketType.NormalCommand:
                    Log.Debug("RESP: Command in place of response");
                    ResponseReceived?.Invoke(this,
                        new CommandPacketResponseReceivedEventArgs(
                            new DeviceResponseException("Invalid response from LCD device -- normal bits set")));
                    break;
                case PacketType.NormalResponse:
                    Log.Debug("RESP: Recognized command response");
                    ResponseReceived?.Invoke(this, new CommandPacketResponseReceivedEventArgs(packet));
                    break;
                case PacketType.NormalReport:
                    if (packet.CommandType == CommandType.KeyActivity)
                    {
                        var action = (KeypadAction)packet.Data[0];
                        Log.Debug("RESP: Keypad event: {KeypadEvent}", action);
                        KeypadActivity?.BeginInvoke(this,
                            new KeypadActivityEventArgs(action.ConvertToKeyFlags(), action), null, null);
                    }
                    // TODO: handle temperature report with event
                    break;
                case PacketType.ErrorResponse:
                    Log.Debug("RESP: Error for {ErrorCommandType}", packet.CommandType);
                    ResponseReceived?.Invoke(this,
                        new CommandPacketResponseReceivedEventArgs(
                            new DeviceResponseException($"Error returned from LCD device for command '{packet.CommandType}'")));
                    break;
                default:
                    Log.Debug("RESP: Unknown response");
                    ResponseReceived?.Invoke(this,
                        new CommandPacketResponseReceivedEventArgs(
                            new DeviceResponseException("Unknown response packet type from LCD device")));
                    break;
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Log.Debug("RECV: Data received begin");
            while (true)
            {
                ReadAllAvailableBytes();
                if (_readBuffer.Count == 0)
                    break;
                var packet = TryParsePacket();
                if (packet == null)
                    break;
                HandleResponsePacket(packet);
            }
            Log.Debug("RECV: Data received complete");
        }

        private static void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            Log.Error("Received serial port error: {PortError}", e.EventType);
        }

        private async Task<CommandPacket> WaitForResponseWithTimeout(CommandType commandType, TimeSpan timeout)
        {
            CommandPacket response = null;
            Exception ex = null;
            var eventWaiter = new TaskCompletionSource<bool>();
            EventHandler<CommandPacketResponseReceivedEventArgs> eventHandler =
                delegate (object sender, CommandPacketResponseReceivedEventArgs args)
                {
                    if (args.Success)
                    {
                        if (args.Response.CommandType != commandType)
                            return;
                        response = args.Response;
                        eventWaiter.SetResult(true);
                    }
                    else
                    {
                        ex = args.DataReceivedException;
                        eventWaiter.SetResult(false);
                    }
                };
            ResponseReceived += eventHandler;
            var completedTask = await Task.WhenAny(eventWaiter.Task, Task.Delay(timeout));
            ResponseReceived -= eventHandler;
            if (ex != null)
                throw ex;
            if (completedTask != eventWaiter.Task)
                throw new DeviceTimeoutException($"A timeout occurred before '{commandType}' command completed");
            return response;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
