using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("setbootstate", HelpText = "Set current state as boot state for device.")]
    internal class SetBootStateOptions : GlobalOptionsBase
    { }

    internal class SetBootStateOp : IOp<SetBootStateOptions>
    {
        public int Run(LcdDevice device, SetBootStateOptions opts)
        {
            device.StoreCurrentStateAsBootState();
            return 0;
        }
    }
}
