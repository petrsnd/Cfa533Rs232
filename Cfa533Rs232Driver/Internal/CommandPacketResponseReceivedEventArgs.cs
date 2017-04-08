namespace petrsnd.Cfa533Rs232Driver.Internal
{
    internal class CommandPacketResponseReceivedEventArgs
    {
        public CommandPacketResponseReceivedEventArgs(CommandPacket response)
        {
            Response = response;
        }

        public CommandPacket Response { get; private set; }
    }
}
