using System;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace petrsnd.Cfa533Rs232Driver.Internal
{
    internal class Cfa533Rs232Connection : IDisposable
    {
        private readonly string _serialPortName;
        private int _baudRate;
        private SerialPort _serialPort;
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
        }

        public bool Connected => _serialPort != null && _serialPort.IsOpen;

        public EventHandler<KeypadEventArgs> KeypadActivity; 

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
                lock (_locker)
                {
                    var commandBuffer = command.ConvertToBuffer();
                    _serialPort.Write(commandBuffer, 0, commandBuffer.Length);
                    using (var t = WaitForResponseWithTimeout(command.CommandType, TimeSpan.FromSeconds(2)))
                    {
                        t.Wait();
                        if (t.Result == null)
                            throw new DeviceTimeoutException("Timeout while waiting for LCD device response");
                        return t.Result;
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
            // TODO: log this stuff I guess...
        }

        private event EventHandler<CommandPacketResponseReceivedEventArgs> ResponseReceived; 

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var numBytes = _serialPort.BytesToRead;
            var recvBuffer = new byte[numBytes];
            _serialPort.Read(recvBuffer, 0, numBytes);
            var packet = CommandPacketParser.Parse(recvBuffer);
            switch (packet.PacketType)
            {
                case PacketType.NormalCommand:
                    throw new DeviceResponseException("Invalid response from LCD device -- normal bits set");
                case PacketType.NormalResponse:
                    ResponseReceived?.Invoke(this, new CommandPacketResponseReceivedEventArgs(packet));
                    break;
                case PacketType.NormalReport:
                    if (packet.CommandType == CommandType.KeyActivity)
                    {
                        var action = (KeypadAction)packet.Data[0];
                        KeypadActivity?.Invoke(this, new KeypadEventArgs(action.ConvertToKeyFlags(), action));
                    }
                    // TODO: handle temperature report with event
                    break;
                case PacketType.ErrorResponse:
                    throw new DeviceResponseException($"Error returned from LCD device '{packet.CommandType}'");
                default:
                    throw new DeviceResponseException("Unknown response packet type from LCD device");
            }
        }

        private static void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            // TODO: log this stuff I guess...
        }

        private async Task<CommandPacket> WaitForResponseWithTimeout(CommandType commandType, TimeSpan timeout)
        {
            CommandPacket response = null;
            var eventWaiter = new TaskCompletionSource<bool>();
            EventHandler<CommandPacketResponseReceivedEventArgs> eventHandler =
                delegate (object sender, CommandPacketResponseReceivedEventArgs args)
                {
                    if (args.Response == null || args.Response.CommandType != commandType)
                        return;
                    response = args.Response;
                    eventWaiter.SetResult(true);
                };
            ResponseReceived += eventHandler;
            var ret = await Task.WhenAny(eventWaiter.Task, Task.Delay(timeout)) == eventWaiter.Task;
            ResponseReceived -= eventHandler;
            return response;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
