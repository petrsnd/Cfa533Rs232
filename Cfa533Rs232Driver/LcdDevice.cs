using System;
using System.Linq;
using System.Text;
using Petrsnd.Cfa533Rs232Driver.Internal;
using Serilog;

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
            if (!Enum.IsDefined(typeof(LcdBaudRate), baudRate))
                throw new ArgumentException($"Invalid baud rate specified, {(int)baudRate}", nameof(baudRate));
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

        private void ThrowIfNotConnected()
        {
            if (!Connected)
                throw new DeviceConnectionException("Device not connected");
        }

        private static void VerifyResponsePacket(CommandPacket response, CommandType expected)
        {
            if (response == null)
            {
                Log.Debug(
                    "Response verification null packet, should only occur when shutting down in mulithreaded appliaction");
                return;
            }
            if (response.PacketType == PacketType.NormalResponse && response.CommandType == expected)
                return;
            Log.Debug(
                "Response verification failed, {PacketType}, Expected: {ExpectedCommand}, Received: {ReceivedCommand}",
                response?.PacketType, expected, response?.CommandType);
            throw new DeviceCommandException(
                $"Invalid response '{response?.Type:X2}' from command '{expected:X2}'");
        }

        public bool Ping(string data)
        {
            ThrowIfNotConnected();
            if (data == null)
                data = "";
            var command = new CommandPacket(CommandType.Ping, (byte)data.Length, Encoding.ASCII.GetBytes(data));
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.Ping);
            return response != null && response.Data.SequenceEqual(command.Data);
        }

        public string GetHardwareFirmwareVersion()
        {
            ThrowIfNotConnected();
            var command = new CommandPacket(CommandType.GetHardwareFirmwareVersion);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.GetHardwareFirmwareVersion);
            return response?.Data == null ? null : Encoding.ASCII.GetString(response.Data);
        }

        public void WriteToUserFlash(byte[] data)
        {
            ThrowIfNotConnected();
            var buffer = Enumerable.Repeat((byte)0x00, 16).ToArray();
            if (data != null)
                Buffer.BlockCopy(data, 0, buffer, 0, Math.Min(data.Length, buffer.Length));
            var command = new CommandPacket(CommandType.WriteToUserFlash, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.WriteToUserFlash);
        }

        public byte[] ReadFromUserFlash()
        {
            ThrowIfNotConnected();
            var command = new CommandPacket(CommandType.ReadFromUserFlash);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.ReadFromUserFlash);
            return response?.Data;
        }

        public void StoreCurrentStateAsBootState()
        {
            ThrowIfNotConnected();
            var command = new CommandPacket(CommandType.StoreCurrentStateAsBootState);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.StoreCurrentStateAsBootState);
        }

        public void SendPowerOperation(PowerOperation op)
        {
            switch (op)
            {
                case PowerOperation.RebootLcd:
                    var data = new byte[] { 0x08, 0x12, 0x63 };
                    var command = new CommandPacket(CommandType.SendPowerOperation, (byte)data.Length, data);
                    var response = _deviceConnection?.SendReceive(command);
                    VerifyResponsePacket(response, CommandType.SendPowerOperation);
                    break;
                case PowerOperation.ResetHost:
                    throw new NotImplementedException();
                case PowerOperation.PowerOffHost:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        public void ClearScreen()
        {
            ThrowIfNotConnected();
            var command = new CommandPacket(CommandType.ClearScreen);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.ClearScreen);
        }

        private static byte[] GetLineAsBuffer(string line)
        {
            var buffer = Enumerable.Repeat((byte)0x20, 16).ToArray();
            if (line == null)
                return buffer;
            var lineBuf = Encoding.ASCII.GetBytes(line);
            Buffer.BlockCopy(lineBuf, 0, buffer, 0, Math.Min(lineBuf.Length, buffer.Length));
            return buffer;
        }

        public void SetLcdLineOneContents(string lineOne)
        {
            ThrowIfNotConnected();
            var buffer = GetLineAsBuffer(lineOne);
            var command = new CommandPacket(CommandType.SetLcdLineOneContents, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetLcdLineOneContents);
        }

        public void SetLcdLineTwoContents(string lineTwo)
        {
            ThrowIfNotConnected();
            var buffer = GetLineAsBuffer(lineTwo);
            var command = new CommandPacket(CommandType.SetLcdLineTwoContents, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetLcdLineTwoContents);
        }

        public void SetSpecialCharacterData(int index, byte[] data)
        {
            if (index < 0 || index > 7)
                throw new ArgumentException("Special character index must be 0 - 7", nameof(index));
            var buffer = Enumerable.Repeat((byte)0x00, 9).ToArray();
            buffer[0] = (byte)index;
            if (data != null)
                Buffer.BlockCopy(data, 0, buffer, 1, Math.Min(data.Length, 7));
            var command = new CommandPacket(CommandType.SetSpecialCharacterData, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetSpecialCharacterData);
        }

        public Tuple<byte, byte[]> ReadMemoryForDebug(byte address)
        {
            if (address < 0x40 || address > 0xCF)
                throw new ArgumentException("Valid addresses are between 0x40 and 0xCF", nameof(address));
            var command = new CommandPacket(CommandType.ReadMemoryForDebug, 1, new[] {address});
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.ReadMemoryForDebug);
            if (response?.Data == null || response.Data.Length < 9)
                throw new DeviceResponseException("Did not receive nine bytes for read memory operation");
            return new Tuple<byte, byte[]>(response.Data[0], response.Data.Skip(1).Take(8).ToArray());
        }

        public void SetCursorPosition(int column, int row)
        {
            if (column < 0 || column > 15)
                throw new ArgumentException("Column index must be 0 - 15", nameof(column));
            if (row < 0 || row > 1)
                throw new ArgumentException("Row index must be 0 - 1", nameof(row));
            var buffer = new[] {(byte)column, (byte)row};
            var command = new CommandPacket(CommandType.SetCursorPosition, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetCursorPosition);
        }

        public void SetCursorStyle(CursorStyle style)
        {
            if (!Enum.IsDefined(typeof(CursorStyle), style))
                throw new ArgumentException("Must specify a valid cursor style", nameof(style));
            var buffer = new[] {(byte)style};
            var command = new CommandPacket(CommandType.SetCursorStyle, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetCursorStyle);
        }

        public void SetContrast(int contrast)
        {
            if (contrast < 0 || contrast > 200)
                throw new ArgumentException("Contrast must be 0 - 200, only 0 - 50 are useful", nameof(contrast));
            var buffer = new[] {(byte)contrast};
            var command = new CommandPacket(CommandType.SetContrast, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetContrast);
        }

        public void SetBacklight(int lcdBrightness, int keypadBrightness)
        {
            if (lcdBrightness < 0 || lcdBrightness > 100)
                throw new ArgumentException("LCD brightness must be 0 - 100", nameof(lcdBrightness));
            if (keypadBrightness < 0 || keypadBrightness > 100)
                throw new ArgumentException("Keypad brightness must be 0 - 100", nameof(lcdBrightness));
            var buffer = new[] {(byte)lcdBrightness, (byte)keypadBrightness};
            var command = new CommandPacket(CommandType.SetBacklight, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetBacklight);
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

        public void SendDataToLcd(int column, int row, string data)
        {
            if (column < 0 || column > 15)
                throw new ArgumentException("Column index must be 0 - 15", nameof(column));
            if (row < 0 || row > 1)
                throw new ArgumentException("Row index must be 0 - 1", nameof(row));
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("Null or empty string data not allowed");
            var length = Math.Min(16 - column, data.Length) + 2;
            var buffer = Enumerable.Repeat((byte)0x20, length).ToArray();
            buffer[0] = (byte)column;
            buffer[1] = (byte)row;
            var dataBuffer = Encoding.ASCII.GetBytes(data);
            Buffer.BlockCopy(dataBuffer, 0, buffer, 2, length - 2);
            var command = new CommandPacket(CommandType.SendDataToLcd, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SendDataToLcd);
        }

        public void SetLcdContents(string lineOne, string lineTwo)
        {
            SendDataToLcd(0, 0, lineOne.PadRight(16, ' '));
            SendDataToLcd(0, 1, lineTwo.PadRight(16, ' '));
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
