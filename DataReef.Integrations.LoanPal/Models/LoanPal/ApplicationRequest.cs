using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.LoanPal.Models.LoanPal
{
    public class ApplicationRequest
    {
        //[JsonProperty("id")]
        //public string Id { get; set; }

        [JsonProperty("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [JsonProperty("applicant")]
        public ApplicantModel Applicant { get; set; }

        [JsonProperty("coApplicants")]
        public List<ApplicantModel> CoApplicants { get; set; }

        [JsonProperty("salesRep")]
        public SalesRepModel SalesRep { get; set; }

        [JsonProperty("totalSystemCost")]
        public string TotalSystemCost { get; set; }

        [JsonProperty("batteryIncluded")]
        public bool BatteryIncluded { get; set; }

        [JsonProperty("selectedLoanOption")]
        public int SelectedLoanOption { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

    }
}
