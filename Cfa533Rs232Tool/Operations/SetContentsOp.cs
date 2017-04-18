using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("setcontents", HelpText = "Set contents of entire LCD.")]
    internal class SetContentsOptions
    {
        [Option(HelpText = "Text for line one of LCD")]
        public string One { get; set; }

        [Option(HelpText = "Text for line two of LCD")]
        public string Two { get; set; }
    }

    internal static class SetContentsOp
    {
        public static int Execute(LcdDevice device, SetContentsOptions opts)
        {
            device.SetLcdContents(opts.One, opts.Two);
            return 0;
        }
    }
}
