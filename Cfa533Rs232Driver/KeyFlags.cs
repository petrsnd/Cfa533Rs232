using System;

namespace Petrsnd.Cfa533Rs232Driver
{
    [Flags]
    public enum KeyFlags : byte
    {
        Up = 0x01,
        Enter = 0x02,
        Cancel = 0x04,
        Left = 0x08,
        Right = 0x10,
        Down = 0x20
    }
}
