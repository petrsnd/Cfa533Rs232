using System;
using CommandLine;
using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool.Operations
{
    [Verb("listen", HelpText = "Listen for communication of events from device.")]
    internal class ListenOptions : GlobalOptionsBase
    { }

    internal class ListenOp : IOp<ListenOptions>
    {
        public int Run(LcdDevice device, ListenOptions opts)
        {
            Console.WriteLine("Press any key to stop listening...");
            EventHandler<KeypadActivityEventArgs> eventHandler =
                delegate (object sender, KeypadActivityEventArgs eventArgs)
                {
                    Console.WriteLine($"Keyboard Event: {eventArgs.KeypadAction}");
                };
            device.KeypadActivity += eventHandler;
            Console.ReadKey();
            device.KeypadActivity -= eventHandler;
            return 0;
        }

    }
}
