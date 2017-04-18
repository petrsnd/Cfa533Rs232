using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("cursorpos", HelpText = "Set the position of the cursor.")]
    internal class CursorPositionOptions
    {
        [Option(HelpText = "Horizontal position / X coordinate / column (0-15)", Required = true)]
        public int Col { get; set; }

        [Option(HelpText = "Vertical position / Y coordinate / row (0-1)", Required = true)]
        public int Row { get; set; }
    }

    internal static class CursorPositionOp
    {
        public static int Execute(LcdDevice device, CursorPositionOptions opts)
        {
            device.SetCursorPosition(opts.Col, opts.Row);
            return 0;
        }
    }
}
