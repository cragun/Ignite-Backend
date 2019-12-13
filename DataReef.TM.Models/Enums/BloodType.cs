using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum BloodType
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        APositive = 1,

        [EnumMember]
        ANegative = 2,

        [EnumMember]
        BPositive = 3,

        [EnumMember]
        BNegative = 4,

        [EnumMember]
        ABPositive = 5,

        [EnumMember]
        ABNegative = 6,

        [EnumMember]
        OPositive = 7,

        [EnumMember]
        ONegative = 8

    }

}
