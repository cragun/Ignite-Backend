using System;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    [Flags]
    public enum Ethnicity
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        White = 1 << 0,

        [EnumMember]
        Hispanic = 1 << 1,

        [EnumMember]
        Asian = 1 << 2,

        [EnumMember]
        AfricanAmerican = 1 << 3,

        [EnumMember]
        NativeAmerican = 1 << 4,

        [EnumMember]
        PacificIslander = 1 << 5,

        [EnumMember]
        Other = 1 << 6,

    }
}
