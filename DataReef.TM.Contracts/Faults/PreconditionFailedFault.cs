using System.Runtime.Serialization;

namespace DataReef.TM.Contracts.Faults
{
    [DataContract]
    public class PreconditionFailedFault
    {
        public PreconditionFailedFault(int code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public string Message { get; set; }

    }
}
