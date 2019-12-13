using DataReef.Core.Classes;
using DataReef.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Attributes
{      
    [AttributeUsage(AttributeTargets.Class)]
    public class MailAttribute: System.Attribute
    {
        public CrudAction CrudAction { get; set; }
        public string MailMethod { get; set; }

    }
}
