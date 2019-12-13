using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum EpcStatusProgress
    {
        [EnumMember]
        Incomplete = 0,

        [EnumMember]
        Complete = 1
    }
}
