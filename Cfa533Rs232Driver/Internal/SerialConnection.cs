using System;
using System.IO.Ports;
using System.Text;

namespace petrsnd.Cfa533Rs232Driver.Internal
{
    class SerialConnection : IDisposable
    {
        private string _serialPortName;
        private int _baudRate;
        private SerialPort _serialPort;

        public SerialConnection(string serialPortName, int baudRate)
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
                // TODO: throw something here
            }
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

        private void ConnectHandlers()
        {
            if (_serialPort == null)
                return;
            _serialPort.PinChanged += null;
            _serialPort.DataReceived += null;
            _serialPort.ErrorReceived += null;
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

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var numBytes = _serialPort.BytesToRead;
            var recvBuffer = new byte[numBytes];
            _serialPort.Read(recvBuffer, 0, numBytes);
            var packet = CommandPacketParser.Parse(recvBuffer);
        }

        private static void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            // TODO: log this stuff I guess...
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
