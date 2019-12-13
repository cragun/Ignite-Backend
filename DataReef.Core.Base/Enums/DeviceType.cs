using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Enums
{
    [DataContract]
    public enum DeviceType
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        iPhone,

        [EnumMember]
        iPad,

        [EnumMember]
        Web,
    }
}
