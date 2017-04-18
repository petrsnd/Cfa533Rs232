using System;
using System.Linq;
using System.Text;
using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Cfa533Rs232Tool
{
    [Verb("writeuserflash", HelpText = "Write binary data to user flash")]
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
                data = Encoding.ASCII.GetBytes(opts.String);
            if (opts.HexString != null)
            {
                var hex =
                    new string(
                        opts.HexString?.Where(c => c >= 0x30 || c <= 0x39 || c >= 0x41 || c <= 0x46 || c >= 0x61 || c <= 0x66)
                            .ToArray());
                if (string.IsNullOrEmpty(hex) || hex.Length % 2 != 0)
                    throw new ArgumentException("Hexidecimal string cannot be interpreted as hex values");
                data =
                    Enumerable.Range(0, hex.Length/2).Select(x => Convert.ToByte(hex.Substring(x*2, 2), 16)).ToArray();
            }
            device.WriteToUserFlash(data);
            return 0;
        }
    }
}
