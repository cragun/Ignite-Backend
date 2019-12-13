using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum PrescreenStatus
    {
        [EnumMember]
        Pending =0,

        [EnumMember]
        InProgress = 1,


        [EnumMember]
        Completed=4,

        [EnumMember]
        Error = 5
    }
}
