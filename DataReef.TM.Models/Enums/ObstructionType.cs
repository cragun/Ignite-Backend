using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum ObstructionType
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        Circle = 1,

        [EnumMember]
        Rectangle = 2,

        [EnumMember]
        Polygon = 3,
    }
}