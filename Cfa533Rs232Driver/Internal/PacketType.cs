namespace petrsnd.Cfa533Rs232Driver.Internal
{
    internal enum PacketType : byte
    {
        NormalCommand = 0x00,
        NormalResponse = 0x01,
        NormalReport = 0x02,
        ErrorResponse = 0x03
    }
}
