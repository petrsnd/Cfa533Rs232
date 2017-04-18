using System;
using System.Linq;
using System.Text;

namespace Cfa533Rs232Tool
{
    internal static class Extensions
    {
        public static byte[] ConvertToBytesAsHexString(this string str)
        {
            var hex =
                    new string(
                        str.Where(c => c >= 0x30 || c <= 0x39 || c >= 0x41 || c <= 0x46 || c >= 0x61 || c <= 0x66)
                            .ToArray());
            if (hex.Length % 2 != 0)
                throw new ArgumentException("String cannot be interpreted as containing hexidecimal values");
            return
                Enumerable.Range(0, hex.Length / 2).Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)).ToArray();
        }

        public static byte[] ConvertToBytesAsAscii(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
    }
}
