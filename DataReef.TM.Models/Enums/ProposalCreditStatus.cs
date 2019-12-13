using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum ProposalCreditStatus
    {
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        Approved = 1,
        [EnumMember]
        Declined = 2
    }
}
