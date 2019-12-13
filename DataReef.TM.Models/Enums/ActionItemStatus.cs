using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum ActionItemStatus
    {
        [EnumMember]
        Unread = 0,

        [EnumMember]
        Read = 1
    }
}
