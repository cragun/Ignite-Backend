using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Microsoft.PowerBI.Models
{
    public class PBI_DispositionChanged : PBI_Base
    {
        public Guid UserID { get; set; }
        public Guid? OUID { get; set; }
        public Guid PropertyID { get; set; }
        public Guid InquiryID { get; set; }
        public string Disposition { get; set; }
        public string IsLead { get; set; }

    }
}
