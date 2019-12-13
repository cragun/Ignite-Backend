using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum Gender
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        Female = 1,

        [EnumMember]
        Male = 2
    }
}
