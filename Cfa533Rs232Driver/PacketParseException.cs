using System;
using System.Runtime.Serialization;

namespace Petrsnd.Cfa533Rs232Driver
{
    public class PacketParseException : Exception
    {
        public PacketParseException()
            : base("Unknown DeviceCommandException")
        {
        }

        public PacketParseException(string message)
            : base(message)
        {
        }

        public PacketParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PacketParseException
            (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
