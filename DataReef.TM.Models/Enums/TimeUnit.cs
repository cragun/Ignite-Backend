using System.Runtime.Serialization;

namespace DataReef.TM.Enums
{
    [DataContract]
    public enum TimeUnit
    {
        [EnumMember]
        Minute =0,

        [EnumMember]
        Hour = 1,

        [EnumMember]
        Day = 2,

        [EnumMember]
        Week = 3,

        [EnumMember]
        SemiWeekly = 4,

        [EnumMember]
        BiMonthly = 5,

        [EnumMember]
        Month = 6

    }
}
