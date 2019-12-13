
namespace DataReef.Integrations.Spruce.DTOs
{
    public class QuoteRequest
    {
        public Init SetupInfo { get; set; }

        public Applicant AppInfo { get; set; }

        public CoApplicant CoAppInfo { get; set; }

        public Employment AppEmployment { get; set; }

        public Employment CoAppEmployment { get; set; }

        public IncomeDebt AppIncomeDebt { get; set; }

        public IncomeDebt CoAppIncomeDebt { get; set; }

        public SpruceProperty Property { get; set; }
    }
}
