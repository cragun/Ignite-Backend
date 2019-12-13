using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Services.Services.ProposalAddons.TriSMART.Models
{
    public class TriSmartConstructor
    {
        public double? UtilityInflationRate { get; set; }
        public ProposalData Data { get; set; }
        public FinancePlan FinancePlan { get; set; }
        public Proposal Proposal { get; set; }
        public LoanRequest Request { get; set; }
        public List<OUSetting> Settings { get; set; }
    }
}
