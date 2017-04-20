using System;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool
{
    internal class OpRunner
    {
        private string _serialPortName = "COM3";
        private int _baudRate = 19200;

        public int Run<TOp, TOpts>(TOpts opts)
            where TOp : IOp<TOpts>, new()
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
                    var demo = new TOp();
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
