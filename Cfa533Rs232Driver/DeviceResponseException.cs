using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace petrsnd.Cfa533Rs232Driver
{
    public class DeviceResponseException : Exception
    {
        public DeviceResponseException()
            : base("Unknown DeviceResponseException")
        {
        }

        public DeviceResponseException(string message)
            : base(message)
        {
        }

        public DeviceResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DeviceResponseException
            (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
