using System.Runtime.Serialization;

namespace DataReef.TM.Contracts.Faults
{
    [DataContract]
    public class InvalidSyncItemFault
    {
        public InvalidSyncItemFault(string type)
        {
            this.Type = type;
        }

        [DataMember]
        public string Type { get; set; }
    }
}
