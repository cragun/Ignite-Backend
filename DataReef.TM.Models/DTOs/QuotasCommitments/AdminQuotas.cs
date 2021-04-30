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
        public List<Types> type { get; set; }
        public IEnumerable<GuidNamePair> user_type { get; set; } 
        public List<QuotaCommitementsDisposition> dispositions { get; set; } 
    }

    public class Types
    { 
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class QuotaCommitementsDisposition
    {
        public string Disposition { get; set; }

        public string DisplayName { get; set; }

        public string Quota { get; set; }
        public string Commitments { get; set; }

        public string TodayQuotas { get; set; }
        public string WeekQuotas { get; set; }
        public string RangeQuotas { get; set; }

        public string TodayCommitments { get; set; }
        public string WeekCommitments { get; set; }
        public string RangeCommitments { get; set; }

    }
}
