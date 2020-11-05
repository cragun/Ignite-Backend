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
