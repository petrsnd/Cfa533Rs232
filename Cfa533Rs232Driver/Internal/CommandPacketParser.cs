using System;

namespace petrsnd.Cfa533Rs232Driver.Internal
{
    internal static class CommandPacketParser
    {
        public static CommandPacket Parse(byte[] data)
        {
            if (data.Length > 0)
            {

                var packet = new CommandPacket((CommandType)data[0]);
                if (data.Length > 1)
                {
                    packet.DataLength = data[1];
                    if (data.Length > packet.PacketSize)
                    {
                        Buffer.BlockCopy(data, 2, packet.Data, 0, packet.DataLength);
                        if (data.Length == packet.PacketSizeWithCrc)
                        {
                            var crc = 0;
                            packet.Crc = 0;
                            return packet;
                        }
                        else
                        {
                            // throw
                        }
                    }

                }
                else
                {
                    // throw
                }
            }
            else
            {
                // throw
            }
            // TODO: remove
            return null;
        }
    }
}
