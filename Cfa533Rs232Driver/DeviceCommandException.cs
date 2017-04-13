using System;
using System.Runtime.Serialization;

namespace petrsnd.Cfa533Rs232Driver
{
    public class DeviceCommandException : Exception
    {
        public DeviceCommandException()
            : base("Unknown DeviceCommandException")
        {
        }

        public DeviceCommandException(string message)
            : base(message)
        {
        }

        public DeviceCommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DeviceCommandException
            (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
