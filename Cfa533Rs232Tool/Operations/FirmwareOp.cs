using System;
using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("firmware", HelpText = "Print hardware and firmware version information.")]
    internal class FirmwareOptions : GlobalOptionsBase
    { }

    internal class FirmwareOp : IOp<FirmwareOptions>
    {
        public int Run(LcdDevice device, FirmwareOptions opts)
        {
            Console.WriteLine(device.GetHardwareFirmwareVersion());
            return 0;
        }
    }
}
