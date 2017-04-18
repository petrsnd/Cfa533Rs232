using System;
using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Cfa533Rs232Tool
{
    [Verb("readuserflash", HelpText = "Read binary data from user flash.")]
    internal class ReadUserFlashOptions
    { }

    internal static class ReadUserFlashOp
    {
        public static int Execute(LcdDevice device, ReadUserFlashOptions opts)
        {
            Console.WriteLine(BitConverter.ToString(device.ReadFromUserFlash()));
            return 0;

        }
    }
}
