using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum SettingValueType
    {
        [EnumMember]
        String = 0,
        [EnumMember]
        Bool = 1,
        [EnumMember]
        JSON = 2,
        [EnumMember]
        Number = 3
    }
}
