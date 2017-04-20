using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("contrast", HelpText = "Set LCD screen contrast.")]
    internal class ContrastOptions : GlobalOptionsBase
    {
        [Option(HelpText = "Value for contrast (0-200), only 0-50 meaningful", Required = true)]
        public int Value { get; set; }
    }

    internal class ContrastOp : IOp<ContrastOptions>
    {
        public int Run(LcdDevice device, ContrastOptions opts)
        {
            device.SetContrast(opts.Value);
            return 0;
        }
    }
}
