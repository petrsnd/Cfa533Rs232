﻿using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("setlinetwo", HelpText = "Set line two contents (deprecated).")]
    internal class SetLineTwoOptions
    {
        [Option(HelpText = "Text to display on line two")]
        public string Text { get; set; }
    }
    internal static class SetLineTwoOp
    {
        public static int Execute(LcdDevice device, SetLineTwoOptions opts)
        {
            device.SetLcdLineTwoContents(opts.Text);
            return 0;
        }
    }
}