using System;
using System.Runtime.Serialization;

namespace petrsnd.Cfa533Rs232Driver
{
    public class DeviceTimeoutException : Exception
    {
        public DeviceTimeoutException()
            : base("Unknown DeviceTimeoutException")
        {
        }

        public DeviceTimeoutException(string message)
            : base(message)
        {
        }

        public DeviceTimeoutException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DeviceTimeoutException
            (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
