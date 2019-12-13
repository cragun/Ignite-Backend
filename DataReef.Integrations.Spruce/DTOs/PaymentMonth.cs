
using Newtonsoft.Json;
namespace DataReef.Integrations.Spruce.DTOs
{
    public class PaymentMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal InterestCharge { get; set; }
        public decimal Principal { get; set; }
        public decimal PaymentBalance { get; set; }
        public decimal UnpaidInternalBalance { get; set; }
        [JsonIgnore]
        public decimal SystemProduction { get; set; }
        [JsonIgnore]
        public decimal BenefitsAndIncentives { get; set; }
        [JsonIgnore]
        public decimal ElectricityBillWithoutSolar { get; set; }
        [JsonIgnore]
        public decimal ElectricityBillWithSolar { get; set; }
        [JsonIgnore]
        public decimal Savings { get; set; }
    }
}
