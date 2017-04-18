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

    internal static class PingOp
    {
        
        public static int Execute(LcdDevice device, PingOptions opts)
        {
            var success = device.Ping(opts.Text);
            Console.WriteLine($"Ping was {(success ? "successful" : "unsuccessful")}.");
            return success ? 0 : 1;

        }
    }
}
