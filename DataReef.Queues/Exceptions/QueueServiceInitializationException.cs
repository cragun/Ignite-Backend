using System;
using System.Runtime.Serialization;

namespace DataReef.Queues.Exceptions
{
    [Serializable]
    public class QueueServiceInitializationException : Exception
    {
        public QueueServiceInitializationException()
        {
        }

        public QueueServiceInitializationException(string message)
            : base(message)
        {
        }

        public QueueServiceInitializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected QueueServiceInitializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}