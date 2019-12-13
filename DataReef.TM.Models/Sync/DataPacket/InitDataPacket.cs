using System.Runtime.Serialization;

namespace DataReef.TM.Models.Sync
{
    /// <summary>
    /// Sync object that contains the initialization object for the client database
    /// </summary>
    [DataContract(IsReference = true)]
    public class InitDataPacket
    {
        [DataMember]
        public BaseDataPacket Items { get; set; }
    }
}
