
namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class LoanSummaryItem
    {
        public string Name { get; set; }

        public int Length { get; set; }

        public decimal Amount { get; set; }

        public float InterestRate { get; set; }

        public decimal AvgMonthlyPayment { get; set; }

        public decimal TotalPayments { get; set; }

    }
}
