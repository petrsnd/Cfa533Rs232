using System;
using System.Runtime.Serialization;

namespace Petrsnd.Cfa533Rs232Driver
{
    public class DeviceConnectionException : Exception
    {
        public DeviceConnectionException()
            : base("Unknown DeviceResponseException")
        {
        }

        public DeviceConnectionException(string message)
            : base(message)
        {
        }

        public DeviceConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DeviceConnectionException
            (SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
        }   
    }
}
