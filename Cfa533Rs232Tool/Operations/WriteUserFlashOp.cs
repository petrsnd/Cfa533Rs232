using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("writeuserflash", HelpText = "Write binary data to user flash.")]
    internal class WriteUserFlashOptions
    {
        [Option(HelpText = "String value to write (16 characters)", Required = false, SetName = "string")]
        public string String { get; set; }

        [Option(HelpText = "Hexidecimal string value to write (16 bytes)", Required = false, SetName = "hex")]
        public string HexString { get; set; }
    }

    internal static class WriteUserFlashOp
    {
        public static int Execute(LcdDevice device, WriteUserFlashOptions opts)
        {
            byte[] data = null;
            if (opts.String != null)
                data = opts.String.ConvertToBytesAsAscii();
            if (opts.HexString != null)
                data = opts.String.ConvertToBytesAsHexString();
            device.WriteToUserFlash(data);
            return 0;
        }
    }
}
