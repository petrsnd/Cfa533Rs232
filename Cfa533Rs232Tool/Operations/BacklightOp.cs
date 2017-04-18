using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("backlight", HelpText = "Set backlight intensity for LCD and keypad.")]
    internal class BacklightOptions
    {
        [Option(HelpText = "Intensity of LCD backlight", Required = true)]
        public int Lcd { get; set; }

        [Option(HelpText = "Intensity of keypad backlight", Required = true)]
        public int Keypad { get; set; }
    }
    internal static class BacklightOp
    {
        public static int Execute(LcdDevice device, BacklightOptions opts)
        {
            device.SetBacklight(opts.Lcd, opts.Keypad);
            return 0;
        }
    }
}
