using CommandLine;

namespace Petrsnd.Cfa533Rs232Tool
{
    internal abstract class GlobalOptionsBase
    {
        [Option(HelpText = "Name of the COM port")]
        public string Com { get; set; }

        [Option(HelpText = "Baud rate for connection (19200 or 115200)")]
        public int BaudRate { get; set; }
    }
}
