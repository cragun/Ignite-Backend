using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.LoanPal.Models
{
    public class LoanCalculatorRequest
    {
        [JsonProperty("loanAmount")]
        public decimal Amount { get; set; }

        [JsonProperty("rate")]
        public float APR { get; set; }

        [JsonProperty("term")]
        public int TermInYears { get; set; }

        [JsonProperty("itcPct")]
        public decimal ITC { get; set; }

        [JsonProperty("includeTable")]
        public bool IncludeTable { get; set; }
    }
}
