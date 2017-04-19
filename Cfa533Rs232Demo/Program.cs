using System;
using CommandLine;
using Petrsnd.Cfa533Rs232Demo.Demos;
using Petrsnd.Cfa533Rs232Driver;
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
            try
            {
                using (var device = new LcdDevice("COM3", LcdBaudRate.Baud19200))
                {
                    device.Connect();
                    return Parser.Default
                        .ParseArguments
                        <MultiFieldOptions, KnightRiderOptions>(
                                args).MapResult(
                                    (MultiFieldOptions opts) => MultiFieldDemo.Execute(device, opts),
                                    (KnightRiderOptions opts) => KnightRiderDemo.Execute(device, opts),
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
