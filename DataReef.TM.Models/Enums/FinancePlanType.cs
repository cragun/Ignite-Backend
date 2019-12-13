
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum FinancePlanType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        PPA = 1,
        [EnumMember]
        Loan = 2,
        [EnumMember]
        Mortgage = 3,
        [EnumMember]
        Cash = 4,
        [EnumMember]
        Pace = 5,
        [EnumMember]
        ReverseMortgage = 6,
        [EnumMember]
        Lease = 7
    }
}
