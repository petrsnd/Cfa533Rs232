using System;
using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Cfa533Rs232Tool
{
    [Verb("ping", HelpText = "Ping the device to ensure communication.")]
    internal class PingOptions
    {
        [Option(HelpText = "Text to send with the ping (limit 16 characters)", Required = false)]
        public string Text { get; set; }
    }

    [Verb("listen", HelpText = "Listen for communication of events from device.")]
    internal class ListenOptions
    { }

    [Verb("firmware", HelpText = "Print hardware and firmware version information.")]
    internal class VersionOptions
    { }

    internal class Program
    {
        public static int Main(string[] args)
        {
            using (var device = new LcdDevice("COM3", LcdBaudRate.Baud19200))
            {
                device.Connect();
                return Parser.Default.ParseArguments<PingOptions, ListenOptions, VersionOptions>(args).MapResult(
                    (PingOptions opts) =>
                    {
                        var success = device.Ping(opts.Text);
                        Console.WriteLine($"Ping was {(success ? "successful" : "unsuccessful")}.");
                        return success ? 0 : 1;
                    },
                    (ListenOptions opts) =>
                    {
                        Console.WriteLine("Press any key to stop listening...");
                        EventHandler<KeypadActivityEventArgs> eventHandler =
                            delegate (object sender, KeypadActivityEventArgs eventArgs)
                            {
                                Console.WriteLine($"Keyboard Event: {eventArgs.KeypadAction}");
                            };
                        device.KeypadActivity += eventHandler;
                        Console.ReadKey();
                        device.KeypadActivity -= eventHandler;
                        return 0;
                    },
                    (VersionOptions opts) =>
                    {
                        Console.WriteLine(device.GetHardwareFirmwareVersion());
                        return 0;
                    },
                    errs => 1);
            }
        }
    }
}
