using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class LoanResponseSunEdison
    {

        public static LoanResponseSunEdison CreateAdvanced(LoanRequestSunEdison definition)
        {
            LoanResponseSunEdison ret = new LoanResponseSunEdison();
            var am = AmortizationTable.Create(definition);
            ret.LCOESummary = LCOESummary.Create(definition, am);
            ret.LoanSummary = LoanSummary.Create(definition, am);
            ret.Years = AmortizationYear.CreateYears(definition, am);
            if (definition.IncludeMonthsInResponse) ret.Months = am.Rows;

            return ret;
        }

        public LCOESummary LCOESummary { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AmortizationMonth> Months { get; set; }

        public List<AmortizationYear> Years { get; set; }

        public LoanSummary LoanSummary { get; set; }

        /// <summary>
        /// The unique identifier for the request as defined by the client request
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }
    }
}
