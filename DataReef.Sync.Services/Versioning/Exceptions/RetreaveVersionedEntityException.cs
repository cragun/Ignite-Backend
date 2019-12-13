using System;
using System.Runtime.Serialization;

namespace DataReef.Sync.Services.Versioning.Exceptions
{
    [Serializable]
    public class RetrieveVersionedEntityException : Exception
    {
        public RetrieveVersionedEntityException()
        {
        }

        public RetrieveVersionedEntityException(string message)
            : base(message)
        {
        }

        public RetrieveVersionedEntityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RetrieveVersionedEntityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}