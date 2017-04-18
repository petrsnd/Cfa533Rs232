using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("setbootstate", HelpText = "Set current state as boot state for device.")]
    internal class SetBootStateOptions
    { }

    internal static class SetBootStateOp
    {
        public static int Execute(LcdDevice device, SetBootStateOptions opts)
        {
            device.StoreCurrentStateAsBootState();
            return 0;
        }
    }
}
