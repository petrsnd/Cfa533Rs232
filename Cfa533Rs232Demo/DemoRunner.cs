using System;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Demo
{
    internal class DemoRunner
    {
        private string _serialPortName = "COM3";
        private int _baudRate = 19200;

        public int Run<TDemo, TOpts>(TOpts opts)
            where TDemo : IDemo<TOpts>, new()
            where TOpts : GlobalOptionsBase
        {
            if (!string.IsNullOrEmpty(opts.Com))
                _serialPortName = opts.Com;
            if (opts.BaudRate != 0)
                _baudRate = opts.BaudRate;
            try
            {
                using (var device = new LcdDevice(_serialPortName, (LcdBaudRate)_baudRate))
                {
                    device.Connect();
                    var demo = new TDemo();
                    return demo.Run(device, opts);
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
