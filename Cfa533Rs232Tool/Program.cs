using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Cfa533Rs232Tool
{
    [Verb("firmware", HelpText = "Print hardware and firmware version information.")]
    internal class FirmwareOptions
    { }

    internal class Program
    {
        public static int Main(string[] args)
        {
            using (var device = new LcdDevice("COM3", LcdBaudRate.Baud19200))
            {
                device.Connect();
                return Parser.Default
                    .ParseArguments
                    <PingOptions, ListenOptions, FirmwareOptions, ReadUserFlashOptions, WriteUserFlashOptions,
                        SetBootStateOptions, RebootOptions, ClearOptions>(
                            args).MapResult(
                                (PingOptions opts) => PingOp.Execute(device, opts),
                                (ListenOptions opts) => ListenOp.Execute(device, opts),
                                (FirmwareOptions opts) => FirmwareOp.Execute(device, opts),
                                (ReadUserFlashOptions opts) => ReadUserFlashOp.Execute(device, opts),
                                (WriteUserFlashOptions opts) => WriteUserFlashOp.Execute(device, opts),
                                (SetBootStateOptions opts) => SetBootStateOp.Execute(device, opts),
                                (RebootOptions opts) => RebootOp.Execute(device, opts),
                                (ClearOptions opts) => ClearOp.Execute(device, opts),
                                errs => 1);
            }
        }
    }
}
