using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("setbaudrate", HelpText = "Set baudrate for LCD device.")]
    internal class SetBaudRateOptions : GlobalOptionsBase
    {
        [Option(HelpText = "New baud rate (19200 or 115200)", Required = true)]
        public int NewBaudRate { get; set; }
    }

    internal class SetBaudRateOp : IOp<SetBaudRateOptions>
    {
        public int Run(LcdDevice device, SetBaudRateOptions opts)
        {
            device.SetBaudRate((LcdBaudRate)opts.NewBaudRate);
            return 0;
        }
    }
}
