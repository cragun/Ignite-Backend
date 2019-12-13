
namespace DataReef.Integrations.Mosaic.Models
{
    public class LoanPricingResponse
    {
        public string PricingQuoteId            { get; set; }

        public string SunEdCustId               { get; set; }

        public string EstimatedAnnualOutput     { get; set; }

        public string GuaranteedAnnualOutput    { get; set; }

        public string ContractID                { get; set; }

        public string SignURL                   { get; set; }
    }
}
