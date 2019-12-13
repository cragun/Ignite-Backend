using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum ReportingPeriod
    {
        [EnumMember]
        AllTime = 0,

        [EnumMember]
        ThisYear,

        [EnumMember]
        ThisMonth,

        [EnumMember]
        ThisWeek,

        [EnumMember]
        Today,

        [EnumMember]
        SpecifiedDay
    }
}
