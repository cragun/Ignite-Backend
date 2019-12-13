using System;

namespace DataReef.Integrations.Spruce.DTOs
{
    public class QuoteResponse
    {
        public long QuoteNumber { get; set; }

        /// <summary>
        /// This property is added because Spruce announced us that they'll change QuoteNumber to QuoteNo. 
        /// We wanted to get ahead and be prepared when they will make the change.
        /// </summary>
        public long QuoteNo { get; set; }

        public string Decision { get; set; }

        public string CreditResponse { get; set; }

        public DateTime? DecisionDateTime { get; set; }

        public decimal AmountFinanced { get; set; }

        public decimal LoanRate { get; set; }

        public int Term { get; set; }

        public decimal IntroRatePayment { get; set; }

        public int IntroTerm { get; set; }

        public decimal MonthlyPayment { get; set; }

        public int GotoTerm { get; set; }

        public string StipulationText { get; set; }

        public decimal MaxApproved { get; set; }
    }
}
