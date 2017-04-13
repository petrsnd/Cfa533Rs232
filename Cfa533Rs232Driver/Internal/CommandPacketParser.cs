using System;

namespace petrsnd.Cfa533Rs232Driver.Internal
{
    internal static class CommandPacketParser
    {
        public static CommandPacket Parse(byte[] data)
        {
            if (data.Length <= 0)
                throw new PacketParseException("Packet is empty");
            var packet = new CommandPacket((CommandType)data[0]);
            if (data.Length <= 1)
                throw new PacketParseException("Packet not long enough to hold data length field");
            packet.DataLength = data[1];
            if (data.Length <= packet.PacketSize)
                throw new PacketParseException("Packet not long enough to hold data length specified");
            Buffer.BlockCopy(data, 2, packet.Data, 0, packet.DataLength);
            if (data.Length != packet.PacketSizeWithCrc)
                throw new PacketParseException("Packet not long enough for CRC value");
            var crc = 0;
            packet.Crc = 0;
            // TODO: check crc
            return packet;
        }
    }
}
