using System;
using System.Runtime.Serialization;

namespace DataReef.Sync.Contracts.Exceptions
{
    [Serializable]
    public class InvalidSessionException : Exception
    {
         public InvalidSessionException()
        {
        }

        public InvalidSessionException(string message)
            : base(message)
        {
        }

        public InvalidSessionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidSessionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        private Guid sessionGuid;

        public InvalidSessionException(Guid sessionGuid)
        {
            this.sessionGuid = sessionGuid;
        }
        
        public Guid SessionGuid
        {
            get { return this.sessionGuid; }
            set { this.sessionGuid = value; }
        }
    }
}
