
namespace DataReef.Integrations.Spruce.DTOs
{
    public class PaymentYear
    {
        public int Year { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal InterestCharge { get; set; }
        public decimal Principal { get; set; }
        public decimal PaymentBalance { get; set; }
        public decimal UnpaidInternalBalance { get; set; }
        public long SystemProduction { get; set; }
        public decimal BenefitsAndIncentives { get; set; }
        public decimal ElectricityBillWithoutSolar { get; set; }
        public decimal ElectricityBillWithSolar { get; set; }
        public decimal Savings { get; set; }
    }
}
