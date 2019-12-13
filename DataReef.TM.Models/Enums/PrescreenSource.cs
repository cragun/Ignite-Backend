using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum PrescreenSource
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        DataReef = 1,

        [EnumMember]
        Spruce = 2,

        [EnumMember]
        Element = 3
    }
}
