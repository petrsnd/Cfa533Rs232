using System;
using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("ping", HelpText = "Ping the device to ensure communication.")]
    internal class PingOptions : GlobalOptionsBase
    {
        [Option(HelpText = "Text to send with the ping (limit 16 characters)", Required = false)]
        public string Text { get; set; }
    }

    internal class PingOp : IOp<PingOptions>
    {
        public int Run(LcdDevice device, PingOptions opts)
        {
            var success = device.Ping(opts.Text);
            Console.WriteLine($"Ping was {(success ? "successful" : "unsuccessful")}.");
            return success ? 0 : 1;

        }
    }
}
