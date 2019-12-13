using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum AdderItemType
    {
        [EnumMember]
        Adder = 0,

        [EnumMember]
        Incentive = 1
    }
}