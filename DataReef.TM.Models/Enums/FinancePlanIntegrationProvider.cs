using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum FinancePlanIntegrationProvider
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        LoanPal = 1,

        [EnumMember]
        Sunnova = 2,

        [EnumMember]
        SalalCreditUnion,

        [EnumMember]
        PaymentFactors,

    }
}
