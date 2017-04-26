using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("setlineone", HelpText = "Set line one contents (deprecated).")]
    internal class SetLineOneOptions : GlobalOptionsBase
    {
        [Option(HelpText = "Text to display on line one")]
        public string Text { get; set; }
    }

    internal class SetLineOneOp : IOp<SetLineOneOptions>
    {
        public int Run(LcdDevice device, SetLineOneOptions opts)
        {
            device.SetLcdLineOneContents(opts.Text);
            return 0;
        }
    }
}
