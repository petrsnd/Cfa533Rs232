using System;

namespace Petrsnd.Cfa533Rs232Driver
{
    public interface ILcdDevice : IDisposable
    {
        /// <summary>
        /// Open serial connection to the LCD device as configured when constructed.
        /// </summary>
        void Connect();

        /// <summary>
        /// Serial connection status.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Close serial connection to LCD device.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Event for receiving keypad activty.  These events will always come on a
        /// separate thread.  Sending commands based on key activity events will have
        /// to wait for a lock.  Because of the packet-based communication of CFA533,
        /// only one command is handled at a time in order to allow each command to
        /// be acknowledged.
        /// </summary>
        event EventHandler<KeypadActivityEventArgs> KeypadActivity;

        /// <summary>
        /// Send a ping to the LCD device to ensure proper packet-based communication.
        /// </summary>
        /// <param name="data">String that will be echoed back from LCD device.</param>
        /// <returns></returns>
        bool Ping(string data);

        /// <summary>
        /// Get the hardware and firmware version of the LCD as a single string.
        /// </summary>
        /// <returns>Hardware and firmware versions.</returns>
        string GetHardwareFirmwareVersion();

        /// <summary>
        /// Write 16 bytes of data to the LCD device user storage area.
        /// </summary>
        /// <param name="data">If more than 16 bytes are specified, only the first 16 will be stored.
        /// If less than 16 bytes are specified, the remaining bytes will be set to 0x00.</param>
        void WriteToUserFlash(byte[] data);

        /// <summary>
        /// Read 16 bytes of data from the LCD device user storage area.
        /// </summary>
        /// <returns>Exactly 16 bytes will always be returned.</returns>
        byte[] ReadFromUserFlash();

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
        void StoreCurrentStateAsBootState();

        /// <summary>
        /// Send a power operation to the LCD device to affect the device itself or the host if it
        /// is configured to do so.
        /// </summary>
        /// <param name="op">Currently the only supported operation is RebootLcd.</param>
        void SendPowerOperation(PowerOperation op);

        /// <summary>
        /// Clear all characters from the LCD device screen.
        /// </summary>
        void ClearScreen();

        /// <summary>
        /// Set the string contents of first line of the LCD device screen.
        /// </summary>
        /// <param name="lineOne">Only the first 16 characters will be displayed.  If a shorter
        /// string is specified, the remaining columns of the screen will be cleared.</param>
        void SetLcdLineOneContents(string lineOne);

        /// <summary>
        /// Set the string contents of second line of the LCD device screen.
        /// </summary>
        /// <param name="lineOne">Only the first 16 characters will be displayed.  If a shorter
        /// string is specified, the remaining columns of the screen will be cleared.</param>
        void SetLcdLineTwoContents(string lineTwo);

        /// <summary>
        /// Set the font definition for one of the special characters (CGRAM).
        /// </summary>
        /// <param name="index">Character index, valid indexes are 0 - 7.</param>
        /// <param name="data">The 8 byte bitmap of the new font for this character.</param>
        void SetSpecialCharacterData(int index, byte[] data);

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
        Tuple<byte, byte[]> ReadMemoryForDebug(byte address);

        /// <summary>
        /// Set the position of the cursor on the LCD device screen.
        /// </summary>
        /// <param name="column">Column index, 0 - 15.</param>
        /// <param name="row">Row index, 0 - 1.</param>
        void SetCursorPosition(int column, int row);

        /// <summary>
        /// Set the cursor style on the LCD device screen.
        /// </summary>
        /// <param name="style">Style to set.</param>
        void SetCursorStyle(CursorStyle style);

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
        void SetContrast(int contrast);

        /// <summary>
        /// Set the brightness of the backlights for the LCD device screen and keypad.
        /// </summary>
        /// <remarks>
        /// The brightness levels affect the lifetime guarantees of the LCD device.
        /// Setting to 0 is off.
        /// </remarks>
        /// <param name="lcdBrightness">The brightness level of the LCD device screen, 0 - 100.</param>
        /// <param name="keypadBrightness">The brightness level of the LCD device keypad, 0 -100.</param>
        void SetBacklight(int lcdBrightness, int keypadBrightness);

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="deviceIndex"></param>
        /// <returns></returns>
        byte[] ReadDowDeviceInformation(int deviceIndex);

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="data"></param>
        void SetUpTemperatureReporting(byte[] data);

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="deviceIndex"></param>
        /// <param name="data"></param>
        void ArbitraryDowTransaction(int deviceIndex, byte[] data);

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
        void SetUpLiveTemperatureDisplay(int displaySlot, TemperatureDisplayItemType type, int deviceIndex,
            int numberOfDigits, int column, int row, TemperatureUnits units);

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        void SendCommandToController(LocationCode code, byte data);

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="pressMask"></param>
        /// <param name="upMask"></param>
        void ConfigureKeyReporting(byte pressMask, byte upMask);

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <returns></returns>
        Tuple<byte, byte, byte> ReadKeypadPolled();

        /// <summary>
        /// Not implemented.
        /// </summary>
        void SetAtxSwitchFunctionality();

        /// <summary>
        /// Not implemented.
        /// </summary>
        void HostWatchdogReset();

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <returns></returns>
        byte[] ReadReportingAtxWatchdog();

        /// <summary>
        /// Send data to the LCD device screen.  Coordinates can be specified to begin
        /// writing.  Only the specified characters are written.  Any characters that
        /// would run off the screen are truncated.
        /// </summary>
        /// <param name="column">Column index, 0 - 15.</param>
        /// <param name="row">Row index, 0 - 1.</param>
        /// <param name="data">String to write at the specified location.</param>
        void SendDataToLcd(int column, int row, string data);

        /// <summary>
        /// Convenience method to set the entire contents of the LCD device screen in
        /// one method call.
        /// </summary>
        /// <param name="lineOne">Only the first 16 characters will be displayed.  If a shorter
        /// string is specified, the remaining columns of the screen will be cleared.</param>
        /// <param name="lineTwo">Only the first 16 characters will be displayed.  If a shorter
        /// string is specified, the remaining columns of the screen will be cleared.</param>
        void SetLcdContents(string lineOne, string lineTwo);

        /// <summary>
        /// Set a new baud rate for communications with the LCD device.
        /// </summary>
        /// <remarks>
        /// Calling this method basically results in a reconnect of the serial port.  The command
        /// is acknowledged and then the connection is reopened with the new baud rate.
        /// </remarks>
        /// <param name="baudRate">Only 9600, 19200, and 115200 are supported</param>
        void SetBaudRate(LcdBaudRate baudRate);

        /// <summary>
        /// Not implemented.
        /// </summary>
        void ConfigureGpio();

        /// <summary>
        /// Not implemented.
        /// </summary>
        void ReadGpioPinLevelsAndState();
    }
}
