using System;

namespace Petrsnd.Cfa533Rs232Driver.Internal
{
    internal class CommandPacketResponseReceivedEventArgs
    {
        public CommandPacketResponseReceivedEventArgs(CommandPacket response)
        {
            Response = response;
        }

        public CommandPacketResponseReceivedEventArgs(Exception ex)
        {
            DataReceivedException = ex;
        }

        public CommandPacket Response { get; private set; }

        public Exception DataReceivedException { get; private set; }

        public bool Success => Response != null && DataReceivedException == null;
    }
}
