using System;
using System.Threading;
using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Demo.Demos
{
    [Verb("knightrider", HelpText = "Character tracing around the screen.")]
    internal class KnightRiderOptions : GlobalOptionsBase
    { }

    internal class KnightRiderDemo : IDemo<KnightRiderOptions>
    {
        public int Run(LcdDevice device, KnightRiderOptions opts)
        {
            device.ClearScreen();
            int x = 0, y = 0;
            using (var timer = new Timer(state =>
            {
                var oldX = x;
                var oldY = y;
                if (y == 0)
                    x+=4;
                else
                    x-=4;
                if (x >= 13)
                {
                    x = 12;
                    y++;
                }
                else if (x < 0)
                {
                    x = 0;
                    y--;
                }
                if (device.Connected)
                {
                    device.SendDataToLcd(x, y, "\0\0\0\0");
                    device.SendDataToLcd(oldX, oldY, "    ");
                }
            }, null, 0, 200))
            {
                Console.WriteLine("Press any key to stop...");
                Console.ReadKey();
                if (device.Connected)
                    device.ClearScreen();
            }
            return 0;
        }
    }
}
