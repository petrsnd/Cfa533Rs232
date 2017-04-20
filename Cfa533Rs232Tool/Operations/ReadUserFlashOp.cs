using System;
using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("readuserflash", HelpText = "Read binary data from user flash.")]
    internal class ReadUserFlashOptions : GlobalOptionsBase
    { }

    internal class ReadUserFlashOp : IOp<ReadUserFlashOptions>
    {
        public int Run(LcdDevice device, ReadUserFlashOptions opts)
        {
            Console.WriteLine(BitConverter.ToString(device.ReadFromUserFlash()));
            return 0;

        }
    }
}
