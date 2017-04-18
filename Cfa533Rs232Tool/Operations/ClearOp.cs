using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("clear", HelpText = "Clear the screen on the device.")]
    internal class ClearOptions
    { }

    internal static class ClearOp
    {
        public static int Execute(LcdDevice device, ClearOptions opts)
        {
            device.ClearScreen();
            return 0;
        }
    }
}
