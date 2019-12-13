using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum AdderItemRateType
    {
        [EnumMember]
        Flat = 0,

        [EnumMember]
        PerKw = 1,

        [EnumMember]
        PerWatt = 2
    }
}