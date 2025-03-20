[![Build status](https://ci.appveyor.com/api/projects/status/belpoe9qhduusy3j?svg=true)](https://ci.appveyor.com/project/petrsnd/cfa533rs232)
[![nuget](https://img.shields.io/nuget/vpre/Cfa533Rs232Driver)](https://www.nuget.org/packages/Cfa533Rs232Driver)
[![License](https://img.shields.io/github/license/petrsnd/Cfa533Rs232)](https://github.com/petrsnd/Cfa533Rs232/blob/master/LICENSE)

# Cfa533Rs232
C# Driver for CrystalFontz CFA533 family of 16x2 LCDs, RS232 interface; Developed against the CFA533-TMI-KU

1.0 update was to support .NET Standard 2.0!

## NuGet
[Cfa533Rs232Driver](https://www.nuget.org/packages/Cfa533Rs232Driver) package on nuget.org.

To install CrystalFontz CFA533 Serial Driver, run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console):
```Powershell
PM> Install-Package Cfa533Rs232Driver
```

## Sources
There are three projects included in the solution:
- Cfa533Rs232Driver -- This is the main library that drives the CFA533 via serial port communications.
- Cfa533Rs232Tool -- A command-line tool for exercising all currently implemented CFA533 commands.
- Cfa533Rs232Demo -- A command-line tool that implements slightly higher-level device interactions.

## Examples
Create an LCD device with the appropriate serial port name.  My CFA533 is usually given 'COM3' and, by default, communicates at 19200 baud rate.
```C#
using (var lcdDevice = new LcdDevice("COM3", LcdBaudRate.Baud19200))
{
    // You must call Connect() before trying to communicate with the device
    lcdDevice.Connect();
    
    // Call lots of methods here...
    lcdDevice.SetLcdContents("Hello", "World!");
}
```

In order to support keypad events you need to register an event handler.
```C#
EventHandler<KeypadActivityEventArgs> eventHandler =
    delegate(object sender, KeypadActivityEventArgs eventArgs)
    {
        Console.WriteLine($"Keyboard Event: {eventArgs.KeypadAction}");
    }
lcdDevice.KeypadActivity += eventHandler;
```

The CFA533 is meant to handle one command at a time.  There is a lock that ensures that only one command is waiting for a response to be acknowledged before additional commands are sent.  However, keypad events may come at any time.  This driver works pretty well in a multithreaded environment, although I have noticed an issue here and there with clean-up where not all threads know that Disconnect() has been called.  In general, this should just produce a DeviceConnectionException stating that the device is not connected.
