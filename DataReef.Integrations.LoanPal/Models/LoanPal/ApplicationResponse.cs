using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.LoanPal.Models.LoanPal
{
    public class ApplicationResponse
    {
        [JsonProperty("loanId")]
        public string LoanId { get; set; }

        [JsonProperty("loanOptions")]
        public List<LoanOptionModel> LoanOptions { get; set; }

        [JsonProperty("maxLoanAmount")]
        public string MaxLoanAmount { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("stipulations")]
        public List<StipulationModel> Stipulations { get; set; }

        [JsonProperty("totalSystemCost")]
        public string TotalSystemCost { get; set; }

        [JsonProperty("errors")]
        public List<LoanPalErrorModel> Errors { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
