using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Classes
{
    public class PropertyAttachmentRejectedTemplate : BaseTemplate
    {
        public string EmailSubject { get; set; }

        public string PropertyName { get; set; }

        public string PropertyAddress { get; set; }

        public IEnumerable<PropertyAttachmentRejectedItemTemplate> RejectedItems { get; set; }
    }

    public class PropertyAttachmentRejectedItemTemplate
    {
        public string SectionName { get; set; }

        public string ItemName { get; set; }

        public string RejectionMessage { get; set; }

        public string ImageUrl { get; set; }
    }
}
