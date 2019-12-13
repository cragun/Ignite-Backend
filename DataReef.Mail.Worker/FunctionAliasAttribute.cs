using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Mail
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=true)]
    public class FunctionAliasAttribute:System.Attribute
    {
        public string Name { get; set; }
    }
}
