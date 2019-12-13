using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum FinanceProviderProposalFlowType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        ServiceFinance = 1,
        [EnumMember]
        LoanPal = 2
    }
}
