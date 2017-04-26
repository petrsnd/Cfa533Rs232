using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("setlinetwo", HelpText = "Set line two contents (deprecated).")]
    internal class SetLineTwoOptions : GlobalOptionsBase
    {
        [Option(HelpText = "Text to display on line two")]
        public string Text { get; set; }
    }

    internal class SetLineTwoOp : IOp<SetLineTwoOptions>
    {
        public int Run(LcdDevice device, SetLineTwoOptions opts)
        {
            device.SetLcdLineTwoContents(opts.Text);
            return 0;
        }
    }
}
