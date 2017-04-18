using System;
using CommandLine;
using Petrsnd.Cfa533Rs232Driver;
using Petrsnd.Cfa533Rs232Tool.Operations;

namespace Petrsnd.Cfa533Rs232Tool
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                using (var device = new LcdDevice("COM3", LcdBaudRate.Baud19200))
                {
                    device.Connect();
                    return Parser.Default
                        .ParseArguments
                        <PingOptions, ListenOptions, FirmwareOptions, ReadUserFlashOptions, WriteUserFlashOptions,
                            SetBootStateOptions, RebootOptions, ClearOptions, SetLineOneOptions, SetLineTwoOptions,
                            CursorPositionOptions, CursorStyleOptions, ContrastOptions, BacklightOptions,
                            SendDataOptions, SetContentsOptions>(
                                args).MapResult(
                                    (PingOptions opts) => PingOp.Execute(device, opts),
                                    (ListenOptions opts) => ListenOp.Execute(device, opts),
                                    (FirmwareOptions opts) => FirmwareOp.Execute(device, opts),
                                    (ReadUserFlashOptions opts) => ReadUserFlashOp.Execute(device, opts),
                                    (WriteUserFlashOptions opts) => WriteUserFlashOp.Execute(device, opts),
                                    (SetBootStateOptions opts) => SetBootStateOp.Execute(device, opts),
                                    (RebootOptions opts) => RebootOp.Execute(device, opts),
                                    (ClearOptions opts) => ClearOp.Execute(device, opts),
                                    (SetLineOneOptions opts) => SetLineOneOp.Execute(device, opts),
                                    (SetLineTwoOptions opts) => SetLineTwoOp.Execute(device, opts),
                                    (CursorPositionOptions opts) => CursorPositionOp.Execute(device, opts),
                                    (CursorStyleOptions opts) => CursorStyleOp.Execute(device, opts),
                                    (ContrastOptions opts) => ContrastOp.Execute(device, opts),
                                    (BacklightOptions opts) => BacklightOp.Execute(device, opts),
                                    (SendDataOptions opts) => SendDataOp.Execute(device, opts),
                                    (SetContentsOptions opts) => SetContentsOp.Execute(device, opts),
                                    errs => 1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
    }
}
