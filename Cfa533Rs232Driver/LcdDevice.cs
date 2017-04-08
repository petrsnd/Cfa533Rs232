using System;
using petrsnd.Cfa533Rs232Driver.Internal;

namespace petrsnd.Cfa533Rs232Driver
{
    public class LcdDevice : IDisposable
    {
        private string _serialPortName;
        private int _baudRate;
        private Cfa533Rs232Connection _deviceConnection;

        public LcdDevice(string serialPortName, LcdBaudRate baudRate)
        {
            _serialPortName = serialPortName;
            _baudRate = ConvertLcdBaudRateToInt(baudRate);
        }

        private int ConvertLcdBaudRateToInt(LcdBaudRate baudRate)
        {
            switch (baudRate)
            {
                case LcdBaudRate.Baud115200:
                    return 115200;
                case LcdBaudRate.Baud19200:
                    return 19200;
                default:
                    return 19200;
            }
        }

        public void Connect()
        {
            if (Connected)
                Disconnect();
        }

        public bool Connected => _deviceConnection != null && _deviceConnection.Connected;

        public void Disconnect()
        {
            if (_deviceConnection == null)
                return;
            _deviceConnection.Disconnect();
            _deviceConnection.Dispose();
            _deviceConnection = null;
        }

        public EventHandler<KeypadEventArgs> KeypadActivity; 

        public bool Ping()
        {
            return _deviceConnection?.SendReceive(new CommandPacket(CommandType.Ping)).PacketType ==
                   PacketType.NormalResponse;
        }

        public string GetHardwareFirmwareVersion()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
