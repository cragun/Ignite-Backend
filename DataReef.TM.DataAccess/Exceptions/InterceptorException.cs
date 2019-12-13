using System;
using System.Runtime.Serialization;

namespace DataReef.TM.DataAccess.Exceptions
{
    [Serializable]
    public class InterceptorException : Exception
    {
        public InterceptorException()
        {
        }

        public InterceptorException(string message)
            : base(message)
        {
        }

        public InterceptorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InterceptorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
