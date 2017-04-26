using CommandLine;
using Petrsnd.Cfa533Rs232Tool.Operations;

namespace Petrsnd.Cfa533Rs232Tool
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default
                .ParseArguments
                <SetBaudRateOptions, PingOptions, ListenOptions, ReadUserFlashOptions, WriteUserFlashOptions,
                SetBootStateOptions, RebootOptions, ClearOptions, SetLineOneOptions, SetLineTwoOptions,
                CursorPositionOptions, CursorStyleOptions, ContrastOptions, BacklightOptions,
                SendDataOptions, SetContentsOptions>(
                    args).MapResult(
                    (SetBaudRateOptions opts) => new OpRunner().Run<SetBaudRateOp, SetBaudRateOptions>(opts),
                    (PingOptions opts) => new OpRunner().Run<PingOp, PingOptions>(opts),
                    (ListenOptions opts) => new OpRunner().Run<ListenOp, ListenOptions>(opts),
                    (ReadUserFlashOptions opts) => new OpRunner().Run<ReadUserFlashOp, ReadUserFlashOptions>(opts),
                    (WriteUserFlashOptions opts) => new OpRunner().Run<WriteUserFlashOp, WriteUserFlashOptions>(opts),
                    (SetBootStateOptions opts) => new OpRunner().Run<SetBootStateOp, SetBootStateOptions>(opts),
                    (RebootOptions opts) => new OpRunner().Run<RebootOp, RebootOptions>(opts),
                    (ClearOptions opts) => new OpRunner().Run<ClearOp, ClearOptions>(opts),
                    (SetLineOneOptions opts) => new OpRunner().Run<SetLineOneOp, SetLineOneOptions>(opts),
                    (SetLineTwoOptions opts) => new OpRunner().Run<SetLineTwoOp, SetLineTwoOptions>(opts),
                    (CursorPositionOptions opts) => new OpRunner().Run<CursorPositionOp, CursorPositionOptions>(opts),
                    (CursorStyleOptions opts) => new OpRunner().Run<CursorStyleOp, CursorStyleOptions>(opts),
                    (ContrastOptions opts) => new OpRunner().Run<ContrastOp, ContrastOptions>(opts),
                    (BacklightOptions opts) => new OpRunner().Run<BacklightOp, BacklightOptions>(opts),
                    (SendDataOptions opts) => new OpRunner().Run<SendDataOp, SendDataOptions>(opts),
                    (SetContentsOptions opts) => new OpRunner().Run<SetContentsOp, SetContentsOptions>(opts),
                    errs => 1);
        }
    }
}
