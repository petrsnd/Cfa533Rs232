using CommandLine;

namespace Petrsnd.Cfa533Rs232Demo
{
    internal abstract class GlobalOptionsBase
    {
        [Option(HelpText = "Name of the COM port")]
        public string Com { get; set; }

        [Option(HelpText = "Baud rate for connection")]
        public int BaudRate { get; set; }
    }
}
