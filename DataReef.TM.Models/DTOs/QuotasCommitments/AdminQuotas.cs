using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.Inquiries;
using DataReef.TM.Models.DTOs.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.QuotasCommitments
{
    public class AdminQuotas
    {
        public List<Type> type { get; set; }
        public IEnumerable<GuidNamePair> user_type { get; set; } 
        public List<CRMDisposition> dispositions { get; set; } 
    }

    public class Type
    { 
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
