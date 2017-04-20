using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("reboot", HelpText = "Reboot device to test boot state.")]
    internal class RebootOptions : GlobalOptionsBase
    { }

    internal class RebootOp : IOp<RebootOptions>
    {
        public int Run(LcdDevice device, RebootOptions opts)
        {
            device.SendPowerOperation(PowerOperation.RebootLcd);
            return 0;
        }
    }
}
