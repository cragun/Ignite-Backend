using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Attributes
{
    public class TemplateNameAttribute : Attribute
    {
        public string TemplateName { get; set; }

        public TemplateNameAttribute(string value)
        {
            TemplateName = value;
        }

    }
}
