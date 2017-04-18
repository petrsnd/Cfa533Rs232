using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("contrast", HelpText = "Set LCD screen contrast.")]
    internal class ContrastOptions
    {
        [Option(HelpText = "Value for contrast (0-200), only 0-50 meaningful", Required = true)]
        public int Value { get; set; }
    }

    internal static class ContrastOp
    {
        public static int Execute(LcdDevice device, ContrastOptions opts)
        {
            device.SetContrast(opts.Value);
            return 0;
        }
    }
}
