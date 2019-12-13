using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Attributes
{
    [Flags]
    public enum BootstrapType
    {
        Unknown =0,
        Core = 1<<0,
        Api= 1<<1
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class BootstrapExcludedAttribute:System.Attribute
    {
        public BootstrapType BootstrapType { get; set; }
    }
}
