using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Classes
{
    public class PropertyAttachmentSubmittedTemplate : BaseTemplate
    {
        public string EmailSubject { get; set; }

        public string PropertyName { get; set; }

        public string Address { get; set; }

        public string AttachmentTypeName { get; set; }
    }
}
