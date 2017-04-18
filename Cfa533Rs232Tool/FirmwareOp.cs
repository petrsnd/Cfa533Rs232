using System;
using Petrsnd.Cfa533Rs232Driver;

namespace Cfa533Rs232Tool
{
    internal static class FirmwareOp
    {
        public static int Execute(LcdDevice device, FirmwareOptions opts)
        {
            Console.WriteLine(device.GetHardwareFirmwareVersion());
            return 0;
        }
    }
}
