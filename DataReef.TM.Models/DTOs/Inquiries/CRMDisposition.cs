using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Inquiries
{
    public class CRMDisposition
    {
        public string Disposition { get; set; }

        public string DisplayName { get; set; }

        public string Color { get; set; }

        public string Icon { get; set; }
        public int? SBTypeId { get; set; }
        //public int Quota { get; set; }
        //public int Commitments { get; set; }
    }
}
