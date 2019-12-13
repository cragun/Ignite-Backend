using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.PubSubMessaging
{
    [Flags]
    public enum EventActionType
    {
        Insert = 1 << 0,
        Update = 1 << 1,
        SoftDelete = 1 << 2,
        HardDelete = 1 << 3,
    }
}
