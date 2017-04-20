using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("cursorstyle", HelpText = "Set cursor style")]
    internal class CursorStyleOptions : GlobalOptionsBase
    {
        [Option(HelpText = "0 - None, 1 - Blink Block, 2 - Underscore, 3 - Blink Underscore", Required = true)]
        public int Style { get; set; }
    }

    internal class CursorStyleOp : IOp<CursorStyleOptions>
    {
        public int Run(LcdDevice device, CursorStyleOptions opts)
        {
            device.SetCursorStyle((CursorStyle)opts.Style);
            return 0;
        }
    }
}
