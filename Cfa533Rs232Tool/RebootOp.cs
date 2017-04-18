using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Cfa533Rs232Tool
{
    [Verb("reboot", HelpText = "Reboot device to test boot state.")]
    internal class RebootOptions
    { }

    internal static class RebootOp
    {
        public static int Execute(LcdDevice device, RebootOptions opts)
        {
            device.SendPowerOperation(PowerOperation.RebootLcd);
            return 0;
        }
    }
}
