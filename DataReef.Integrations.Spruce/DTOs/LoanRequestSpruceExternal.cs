
namespace DataReef.Integrations.Spruce.DTOs
{
    public class LoanRequestSpruceExternal
    {
        public string rateType { get; set; }
        public int daysDeferred { get; set; }
        public int introTerm { get; set; }
        public decimal amountFinanced { get; set; }
        public int term { get; set; }
        public decimal apr { get; set; }
        public decimal prepayPct { get; set; }
        public decimal prepayAmt { get; set; }
        public bool addPaymentOptions { get; set; }

        public static LoanRequestSpruceExternal FromRequest(LoanRequestSpruce request, decimal amountFinanced)
        {
            return new LoanRequestSpruceExternal
            {
                rateType = request.RateType,
                daysDeferred  = request.DaysDeferred,
                introTerm = request.IntroTerm,
                amountFinanced = amountFinanced,
                term = request.Term,
                apr = request.Apr,
                prepayPct = 0,
                prepayAmt = (request.FederalTaxIncentivePercentage * amountFinanced) + request.StateTaxIncentive,
                addPaymentOptions = request.AddPaymentOptions
            };
        }
    }
}
