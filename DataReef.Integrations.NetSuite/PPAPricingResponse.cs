using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataReef.Integrations.NetSuite
{
    public class PPAPricingResponse
    {
        public string PricingQuoteId            { get; set; }

        public string DownPayment               { get; set; }

        public string LeaseTerm                 { get; set; }

        public string SunEdCustId               { get; set; }

        public string PremiumRatioTest          { get; set; }

        public string EstimatedAnnualOutput     { get; set; }

        public string DealerASP                 { get; set; }

        public List<string> TerminationValues       { get; set; }

        public string UniqueFinancialRunId      { get; set; }

        public string CallVersionId             { get; set; }

        public string Timestamp                 { get; set; }

        public string GuaranteedAnnualOutput    { get; set; }

        public List<string> CustomerPPAPayments { get; set; }

        [JsonProperty(PropertyName = "DealerASP/W")]
        public string DealerASPW                { get; set; }

        public string MaxPPARate                { get; set; }

        public string FinancialModelVersion     { get; set; }

        public string ContractID { get; set; }

        public string SignURL { get; set; }
    }

}
