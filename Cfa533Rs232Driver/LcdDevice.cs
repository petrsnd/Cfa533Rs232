using System;
using System.Linq;
using System.Text;
using Petrsnd.Cfa533Rs232Driver.Internal;

namespace Petrsnd.Cfa533Rs232Driver
{
    public class LcdDevice : IDisposable
    {
        private readonly string _serialPortName;
        private int _baudRate;
        private Cfa533Rs232Connection _deviceConnection;

        public LcdDevice(string serialPortName, LcdBaudRate baudRate)
        {
            _serialPortName = serialPortName;
            _baudRate = ConvertLcdBaudRateToInt(baudRate);
        }

        private static int ConvertLcdBaudRateToInt(LcdBaudRate baudRate)
        {
            return (int)baudRate;
        }

        public void Connect()
        {
            if (Connected)
                Disconnect();
            _deviceConnection = new Cfa533Rs232Connection(_serialPortName, _baudRate);
            _deviceConnection.Connect();
            _deviceConnection.KeypadActivity += KeyboardActivityProxy;
        }

        public bool Connected => _deviceConnection != null && _deviceConnection.Connected;

        public void Disconnect()
        {
            if (_deviceConnection == null)
                return;
            _deviceConnection.KeypadActivity -= KeyboardActivityProxy;
            _deviceConnection.Disconnect();
            _deviceConnection.Dispose();
            _deviceConnection = null;
        }

        public event EventHandler<KeypadActivityEventArgs> KeypadActivity;

        private void KeyboardActivityProxy(object sender, KeypadActivityEventArgs args)
        {
            KeypadActivity?.Invoke(sender, args);
        }

        public bool Ping(string data)
        {
            if (!Connected)
                return false;
            if (data == null)
                data = "";
            var command = new CommandPacket(CommandType.Ping, (byte) data.Length, Encoding.ASCII.GetBytes(data));
            var response = _deviceConnection?.SendReceive(command);
            return response?.PacketType == PacketType.NormalResponse && response.CommandType == CommandType.Ping &&
                   response.Data.SequenceEqual(command.Data);
        }

        public string GetHardwareFirmwareVersion()
        {
            if (!Connected)
                throw new DeviceConnectionException("Device not connected");
            var command = new CommandPacket(CommandType.GetHardwareFirmwareVersion);
            var response = _deviceConnection?.SendReceive(command);
            return response?.Data == null ? null : Encoding.ASCII.GetString(response.Data);
        }

        public void WriteToUserFlash(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadFromUserFlash()
        {
            throw new NotImplementedException();
        }

        public void StoreCurrentStateAsBootState()
        {
            throw new NotImplementedException();
        }

        public void SendPowerOperation(PowerOperation op)
        {
            throw new NotImplementedException();
        }

        public void ClearScreen()
        {
            throw new NotImplementedException();
        }

        public void SetScreenLineOneContents(string lineOne)
        {
            throw new NotImplementedException();
        }

        public void SetScreenLineTwoContents(string lineTwo)
        {
            throw new NotImplementedException();
        }

        public void SetSpecialCharacterData(int index, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void ReadMemoryForDebug(byte address)
        {
            throw new NotImplementedException();
        }

        public void SetCursorPosition(int column, int row)
        {
            throw new NotImplementedException();
        }

        public void SetCursorStyle(CursorStyle style)
        {
            throw new NotImplementedException();
        }

        public void SetContrast(int contrast)
        {
            throw new NotImplementedException();
        }

        public void SetBacklight(int lcdBrightness, int keypadBrightness)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadDowDeviceInformation(int deviceIndex)
        {
            throw new NotImplementedException();
        }

        public void SetUpTemperatureReporting(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void ArbitraryDowTransaction(int deviceIndex, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SetUpLiveTemperatureDisplay(int displaySlot, TemperatureDisplayItemType type, int deviceIndex,
            int numberOfDigits, int column, int row, TemperatureUnits units)
        {
            throw new NotImplementedException();
        }

        public void SendCommandToController(LocationCode code, byte data)
        {
            throw new NotImplementedException();
        }

        public void ConfigureKeyReporting(byte pressMask, byte upMask)
        {
            throw new NotImplementedException();
        }

        public Tuple<byte, byte, byte> ReadKeypadPolled()
        {
            throw new NotImplementedException();
        }

        public void SetAtxSwitchFunctionality()
        {
            throw new NotImplementedException();
        }

        public void HostWatchdogReset()
        {
            throw new NotImplementedException();
        }

        public byte[] ReadReportingAtxWatchdog()
        {
            throw new NotImplementedException();
        }

        public void SendDataToScreen(int column, int row, string data)
        {
            throw new NotImplementedException();
        }

        public void SetBaudRate(LcdBaudRate baudRate)
        {
            _baudRate = ConvertLcdBaudRateToInt(baudRate);
            throw new NotImplementedException();
        }

        public void ConfigureGpio()
        {
            throw new NotImplementedException();
        }

        public void ReadGpioPinLevelsAndState()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
