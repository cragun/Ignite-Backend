using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum AdderItemRecurrenceType
    {
        [EnumMember]
        OneTime = 0,
        [EnumMember]
        Monthly,
        [EnumMember]
        Yearly
    }
}
