using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum ProposalStatus
    {
        [EnumMember]
        NotSigned = 0,

        [EnumMember]
        PendingSigning = 1,

        [EnumMember]
        Signed = 2
    }
}
