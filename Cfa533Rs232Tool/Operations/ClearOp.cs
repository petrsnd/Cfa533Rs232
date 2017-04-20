using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("clear", HelpText = "Clear the screen on the device.")]
    internal class ClearOptions : GlobalOptionsBase
    { }

    internal class ClearOp : IOp<ClearOptions>
    {
        public int Run(LcdDevice device, ClearOptions opts)
        {
            device.ClearScreen();
            return 0;
        }
    }
}
