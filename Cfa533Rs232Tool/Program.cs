using System;
using System.Linq;
using petrsnd.Cfa533Rs232Driver;

namespace Cfa533Rs232Tool
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            if (args.Any())
            {
                using (var device = new LcdDevice("COM3", LcdBaudRate.Baud9600))
                {
                    device.Connect();
                    switch (args[0])
                    {
                        case "ping":
                            var success = device.Ping();
                            Console.WriteLine($"Ping was {(success ? "successful" : "unsuccessful")}");
                            return success ? 0 : 1;
                        case "listen":
                            Console.WriteLine("Press any key to stop listening...");
                            Console.ReadKey();
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
