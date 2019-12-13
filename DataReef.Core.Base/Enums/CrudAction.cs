using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Enums
{
    [Flags]
    public enum CrudAction
    {
        Any = 0,
        Insert = 1,
        Update = 1 << 1,
        Delete = 1 << 2
    }

}
