using System;
using System.Linq;
using System.Text;
using Petrsnd.Cfa533Rs232Driver.Internal;
using Serilog;

namespace Petrsnd.Cfa533Rs232Driver
{
    public class LcdDevice : ILcdDevice
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

        /// <summary>
        /// Open serial connection to the LCD device as configured when constructed.
        /// </summary>
        public void Connect()
        {
            if (Connected)
                Disconnect();
            _deviceConnection = new Cfa533Rs232Connection(_serialPortName, _baudRate);
            _deviceConnection.Connect();
            _deviceConnection.KeypadActivity += KeyboardActivityProxy;
        }

        /// <summary>
        /// Serial connection status.
        /// </summary>
        public bool Connected => _deviceConnection != null && _deviceConnection.Connected;

        /// <summary>
        /// Close serial connection to LCD device.
        /// </summary>
        public void Disconnect()
        {
            if (_deviceConnection == null)
                return;
            _deviceConnection.KeypadActivity -= KeyboardActivityProxy;
            _deviceConnection.Disconnect();
            _deviceConnection.Dispose();
            _deviceConnection = null;
        }

        /// <summary>
        /// Event for receiving keypad activty.  These events will always come on a
        /// separate thread.  Sending commands based on key activity events will have
        /// to wait for a lock.  Because of the packet-based communication of CFA533,
        /// only one command is handled at a time in order to allow each command to
        /// be acknowledged.
        /// </summary>
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
                response.PacketType, expected, response.CommandType);
            throw new DeviceCommandException(
                $"Invalid response '{response.Type:X2}' from command '{expected:X2}'");
        }

        /// <summary>
        /// Send a ping to the LCD device to ensure proper packet-based communication.
        /// </summary>
        /// <param name="data">String that will be echoed back from LCD device.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the hardware and firmware version of the LCD as a single string.
        /// </summary>
        /// <returns>Hardware and firmware versions.</returns>
        public string GetHardwareFirmwareVersion()
        {
            ThrowIfNotConnected();
            var command = new CommandPacket(CommandType.GetHardwareFirmwareVersion);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.GetHardwareFirmwareVersion);
            return response?.Data == null ? null : Encoding.ASCII.GetString(response.Data);
        }

        /// <summary>
        /// Write 16 bytes of data to the LCD device user storage area.
        /// </summary>
        /// <param name="data">If more than 16 bytes are specified, only the first 16 will be stored.
        /// If less than 16 bytes are specified, the remaining bytes will be set to 0x00.</param>
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

        /// <summary>
        /// Read 16 bytes of data from the LCD device user storage area.
        /// </summary>
        /// <returns>Exactly 16 bytes will always be returned.</returns>
        public byte[] ReadFromUserFlash()
        {
            ThrowIfNotConnected();
            var command = new CommandPacket(CommandType.ReadFromUserFlash);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.ReadFromUserFlash);
            return response?.Data;
        }

        /// <summary>
        /// Store the current state of the LCD device as its boot state.  This is the state of the
        /// LCD device immediately after power is connected.  Set up desired boot state using other
        /// methods and call this method to store it.
        /// </summary>
        /// <remarks>
        /// State includes: 1) characters on the LCD, 2) special character definitions, 3) cursor
        /// position, 4) cursor style, 5) LCD contrast, 6) LCD backlight brightness, 7) keypad
        /// backlight brightness, 8) settings of any live displays, 9) key press and release masks,
        /// 10) ATX function enable and pulse settings, 11) baud rate, and 12) GPIO settings.
        /// </remarks>
        public void StoreCurrentStateAsBootState()
        {
            ThrowIfNotConnected();
            var command = new CommandPacket(CommandType.StoreCurrentStateAsBootState);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.StoreCurrentStateAsBootState);
        }

        /// <summary>
        /// Send a power operation to the LCD device to affect the device itself or the host if it
        /// is configured to do so.
        /// </summary>
        /// <param name="op">Currently the only supported operation is RebootLcd.</param>
        public void SendPowerOperation(PowerOperation op)
        {
            ThrowIfNotConnected();
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

        /// <summary>
        /// Clear all characters from the LCD device screen.
        /// </summary>
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

        /// <summary>
        /// Set the string contents of first line of the LCD device screen.
        /// </summary>
        /// <param name="lineOne">Only the first 16 characters will be displayed.  If a shorter
        /// string is specified, the remaining columns of the screen will be cleared.</param>
        public void SetLcdLineOneContents(string lineOne)
        {
            ThrowIfNotConnected();
            var buffer = GetLineAsBuffer(lineOne);
            var command = new CommandPacket(CommandType.SetLcdLineOneContents, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetLcdLineOneContents);
        }

        /// <summary>
        /// Set the string contents of second line of the LCD device screen.
        /// </summary>
        /// <param name="lineTwo">Only the first 16 characters will be displayed.  If a shorter
        /// string is specified, the remaining columns of the screen will be cleared.</param>
        public void SetLcdLineTwoContents(string lineTwo)
        {
            ThrowIfNotConnected();
            var buffer = GetLineAsBuffer(lineTwo);
            var command = new CommandPacket(CommandType.SetLcdLineTwoContents, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetLcdLineTwoContents);
        }

        /// <summary>
        /// Set the font definition for one of the special characters (CGRAM).
        /// </summary>
        /// <param name="index">Character index, valid indexes are 0 - 7.</param>
        /// <param name="data">The 8 byte bitmap of the new font for this character.</param>
        public void SetSpecialCharacterData(int index, byte[] data)
        {
            ThrowIfNotConnected();
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

        /// <summary>
        /// Read contents of the LCD device's DDRAM or CGRAM.  This command is intended for
        /// debugging.
        /// </summary>
        /// <remarks>
        /// 0x40 - 0x7F -- CGRAM
        /// 0x80 - 0x8F -- DDRAM, line 1
        /// 0xC0 - 0xCF -- DDRAM, line 2
        /// </remarks>
        /// <param name="address">Native address code of the desired data.</param>
        /// <returns>8 bytes of memory from the specified location.</returns>
        public Tuple<byte, byte[]> ReadMemoryForDebug(byte address)
        {
            ThrowIfNotConnected();
            if (address < 0x40 || address > 0xCF)
                throw new ArgumentException("Valid addresses are between 0x40 and 0xCF", nameof(address));
            var command = new CommandPacket(CommandType.ReadMemoryForDebug, 1, new[] {address});
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.ReadMemoryForDebug);
            if (response?.Data == null || response.Data.Length < 9)
                throw new DeviceResponseException("Did not receive nine bytes for read memory operation");
            return new Tuple<byte, byte[]>(response.Data[0], response.Data.Skip(1).Take(8).ToArray());
        }

        /// <summary>
        /// Set the position of the cursor on the LCD device screen.
        /// </summary>
        /// <param name="column">Column index, 0 - 15.</param>
        /// <param name="row">Row index, 0 - 1.</param>
        public void SetCursorPosition(int column, int row)
        {
            ThrowIfNotConnected();
            if (column < 0 || column > 15)
                throw new ArgumentException("Column index must be 0 - 15", nameof(column));
            if (row < 0 || row > 1)
                throw new ArgumentException("Row index must be 0 - 1", nameof(row));
            var buffer = new[] {(byte)column, (byte)row};
            var command = new CommandPacket(CommandType.SetCursorPosition, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetCursorPosition);
        }

        /// <summary>
        /// Set the cursor style on the LCD device screen.
        /// </summary>
        /// <param name="style">Style to set.</param>
        public void SetCursorStyle(CursorStyle style)
        {
            ThrowIfNotConnected();
            if (!Enum.IsDefined(typeof(CursorStyle), style))
                throw new ArgumentException("Must specify a valid cursor style", nameof(style));
            var buffer = new[] {(byte)style};
            var command = new CommandPacket(CommandType.SetCursorStyle, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetCursorStyle);
        }

        /// <summary>
        /// Set the contrast or vertical viewing angle of the display.
        /// </summary>
        /// <remarks>
        /// Values of 0 - 200 are valid but only 0 - 50 are useful:
        /// 0 is light
        /// 16 is about right
        /// 29 is dark
        /// 30 - 50 are very dark
        /// </remarks>
        /// <param name="contrast">Integer value of the contrast, 0 - 50.</param>
        public void SetContrast(int contrast)
        {
            ThrowIfNotConnected();
            if (contrast < 0 || contrast > 200)
                throw new ArgumentException("Contrast must be 0 - 200, only 0 - 50 are useful", nameof(contrast));
            var buffer = new[] {(byte)contrast};
            var command = new CommandPacket(CommandType.SetContrast, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetContrast);
        }

        /// <summary>
        /// Set the brightness of the backlights for the LCD device screen and keypad.
        /// </summary>
        /// <remarks>
        /// The brightness levels affect the lifetime guarantees of the LCD device.
        /// Setting to 0 is off.
        /// </remarks>
        /// <param name="lcdBrightness">The brightness level of the LCD device screen, 0 - 100.</param>
        /// <param name="keypadBrightness">The brightness level of the LCD device keypad, 0 -100.</param>
        public void SetBacklight(int lcdBrightness, int keypadBrightness)
        {
            ThrowIfNotConnected();
            if (lcdBrightness < 0 || lcdBrightness > 100)
                throw new ArgumentException("LCD brightness must be 0 - 100", nameof(lcdBrightness));
            if (keypadBrightness < 0 || keypadBrightness > 100)
                throw new ArgumentException("Keypad brightness must be 0 - 100", nameof(lcdBrightness));
            var buffer = new[] {(byte)lcdBrightness, (byte)keypadBrightness};
            var command = new CommandPacket(CommandType.SetBacklight, (byte)buffer.Length, buffer);
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetBacklight);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="deviceIndex"></param>
        /// <returns></returns>
        public byte[] ReadDowDeviceInformation(int deviceIndex)
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="data"></param>
        public void SetUpTemperatureReporting(byte[] data)
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="deviceIndex"></param>
        /// <param name="data"></param>
        public void ArbitraryDowTransaction(int deviceIndex, byte[] data)
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="displaySlot"></param>
        /// <param name="type"></param>
        /// <param name="deviceIndex"></param>
        /// <param name="numberOfDigits"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <param name="units"></param>
        public void SetUpLiveTemperatureDisplay(int displaySlot, TemperatureDisplayItemType type, int deviceIndex,
            int numberOfDigits, int column, int row, TemperatureUnits units)
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public void SendCommandToController(LocationCode code, byte data)
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="pressMask"></param>
        /// <param name="upMask"></param>
        public void ConfigureKeyReporting(byte pressMask, byte upMask)
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <returns></returns>
        public Tuple<byte, byte, byte> ReadKeypadPolled()
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void SetAtxSwitchFunctionality()
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void HostWatchdogReset()
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <returns></returns>
        public byte[] ReadReportingAtxWatchdog()
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send data to the LCD device screen.  Coordinates can be specified to begin
        /// writing.  Only the specified characters are written.  Any characters that
        /// would run off the screen are truncated.
        /// </summary>
        /// <param name="column">Column index, 0 - 15.</param>
        /// <param name="row">Row index, 0 - 1.</param>
        /// <param name="data">String to write at the specified location.</param>
        public void SendDataToLcd(int column, int row, string data)
        {
            ThrowIfNotConnected();
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

        /// <summary>
        /// Convenience method to set the entire contents of the LCD device screen in
        /// one method call.
        /// </summary>
        /// <param name="lineOne">Only the first 16 characters will be displayed.  If a shorter
        /// string is specified, the remaining columns of the screen will be cleared.</param>
        /// <param name="lineTwo">Only the first 16 characters will be displayed.  If a shorter
        /// string is specified, the remaining columns of the screen will be cleared.</param>
        public void SetLcdContents(string lineOne, string lineTwo)
        {
            ThrowIfNotConnected();
            if (lineOne == null)
                lineOne = "";
            if (lineTwo == null)
                lineTwo = "";
            SendDataToLcd(0, 0, lineOne.PadRight(16, ' '));
            SendDataToLcd(0, 1, lineTwo.PadRight(16, ' '));
        }

        /// <summary>
        /// Set a new baud rate for communications with the LCD device.
        /// </summary>
        /// <remarks>
        /// Calling this method basically results in a reconnect of the serial port.  The command
        /// is acknowledged and then the connection is reopened with the new baud rate.
        /// </remarks>
        /// <param name="baudRate">Only 9600, 19200, and 115200 are supported</param>
        public void SetBaudRate(LcdBaudRate baudRate)
        {
            ThrowIfNotConnected();
            _baudRate = ConvertLcdBaudRateToInt(baudRate);
            var command = new CommandPacket(CommandType.SetBaudRate, 1,
                new[] {(byte) (baudRate == LcdBaudRate.Baud19200 ? 0 : 1)});
            var response = _deviceConnection?.SendReceive(command);
            VerifyResponsePacket(response, CommandType.SetBaudRate);
            Disconnect();
            Connect();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void ConfigureGpio()
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void ReadGpioPinLevelsAndState()
        {
            ThrowIfNotConnected();
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
