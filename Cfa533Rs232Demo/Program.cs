using System;
using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Demo
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                using (var device = new LcdDevice("COM3", LcdBaudRate.Baud19200))
                {
                    device.Connect();/*
                    return Parser.Default
                        .ParseArguments
                        <>(
                                args).MapResult(
                                    ,
                                    errs => 1);*/
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
    }
}
