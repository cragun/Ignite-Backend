using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum PersonType
    {
        [EnumMember]
        Regular = 0,

        [EnumMember]
        TestUser
    }
}
