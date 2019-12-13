
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum AdderItemReducedAmountType
    {
        [EnumMember]
        Flat = 0,

        [EnumMember]
        Percentage = 1
    }
}