using CommandLine;
using Petrsnd.Cfa533Rs232Demo.Demos;
using Serilog;

namespace Petrsnd.Cfa533Rs232Demo
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .WriteTo.LiterateConsole(outputTemplate:
                    "{Timestamp:HH:mm:ss.ffff} [{Level:u3}] ({ThreadId}) {Message}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .CreateLogger();
            Log.Debug("Debug mode on");
            return Parser.Default
                .ParseArguments
                <MultiFieldOptions, KnightRiderOptions>(
                    args).MapResult(
                    (MultiFieldOptions opts) => new DemoRunner().Run<MultiFieldDemo, MultiFieldOptions>(opts),
                    (KnightRiderOptions opts) => new DemoRunner().Run<KnightRiderDemo, KnightRiderOptions>(opts),
                    errs => 1);

        }
    }
}
