using System;
using System.Linq;
using Petrsnd.Cfa533Rs232Driver;

namespace Cfa533Rs232Tool
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            if (args.Any())
            {
                using (var device = new LcdDevice("COM3", LcdBaudRate.Baud19200))
                {
                    device.Connect();
                    switch (args[0])
                    {
                        case "ping":
                            var success = device.Ping(args.Length > 1 ? args[1] : "");
                            Console.WriteLine($"Ping was {(success ? "successful" : "unsuccessful")}");
                            return success ? 0 : 1;
                        case "listen":
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
                        default:
                            Console.WriteLine($"Unrecognized command '{args[0]}'");
                            return 1;
                    }
                }
            }
            else
            {
                Console.WriteLine("Support commands: ping, listen");
                return 0;
            }
        }
    }
}
