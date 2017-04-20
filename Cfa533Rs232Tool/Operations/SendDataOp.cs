using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("senddata", HelpText = "Send data to position on LCD screen.")]
    internal class SendDataOptions : GlobalOptionsBase
    {
        [Option(HelpText = "Horizontal position / X coordinate / column (0-15)", Required = true)]
        public int Col { get; set; }

        [Option(HelpText = "Vertical position / Y coordinate / row (0-1)", Required = true)]
        public int Row { get; set; }

        [Option(HelpText = "Text to display at position", Required = true)]
        public string Text { get; set; }
    }

    internal class SendDataOp : IOp<SendDataOptions>
    {
        public int Run(LcdDevice device, SendDataOptions opts)
        {
            device.SendDataToLcd(opts.Col, opts.Row, opts.Text);
            return 0;
        }
    }
}
