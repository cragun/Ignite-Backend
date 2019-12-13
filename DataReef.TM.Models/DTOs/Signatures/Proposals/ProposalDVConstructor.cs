using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    public class ProposalDVConstructor
    {
        public ProposalData Data { get; set; }
        public string DealerName { get; set; }
        public Proposal Proposal { get; set; }
        public FinancePlan FinancePlan { get; set; }
        public LoanRequest Request { get; set; }
        public LoanResponse Response { get; set; }
        /// <summary>
        /// Number of years used to generate the Forecast data
        /// </summary>
        public int ForecastTermInYears { get; set; } = 30;
        public double? UtilityInflationRate { get; set; }
        public string ContractorID { get; set; }
        public DateTime DeviceDate { get; set; }
    }
}
