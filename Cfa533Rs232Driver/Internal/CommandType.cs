namespace Petrsnd.Cfa533Rs232Driver.Internal
{
    internal enum CommandType : byte
    {
        Ping = 0x00,
        GetHardwareFirmwareVersion = 0x01,
        WriteToUserFlash = 0x02,
        ReadFromUserFlash = 0x03,
        StoreCurrentStateAsBootState = 0x04,
        SendPowerOperation = 0x05,
        ClearScreen = 0x06,
        SetScreenLineOneContents = 0x07,
        SetScreenLineTwoContents = 0x08,
        SetSpecialCharacterData = 0x09,
        ReadMemoryForDebug = 0x0A,
        SetCursorPosition = 0x0B,
        SetCursorStyle = 0x0C,
        SetContrast = 0x0D,
        SetBacklight = 0x0E,
        ReadDowDeviceInformation = 0x12,
        SetUpTemperatureReporting = 0x13,
        ArbitraryDowTransaction = 0x14,
        SetUpLiveTemperatureDisplay = 0x15,
        SendCommandToController = 0x16,
        ConfigureKeyReporting = 0x17,
        ReadKeypadPolled = 0x18,
        SetAtxSwitchFunctionality = 0x1C,
        HostWatchdogReset = 0x1D,
        ReadReportingAtxWatchdog = 0x1E,
        SendDataToScreen = 0x1F,
        SetBaudRate = 0x21,
        ConfigureGpio = 0x22,
        ReadGpioPinLevelsAndState = 0x23,
        // Report Codes
        KeyActivity = 0x80,
        TemperatureSensorReport = 0x82
    }
}
