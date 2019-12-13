using RestSharp.Deserializers;
using System.Collections.Generic;

namespace DataReef.Integrations.Spruce.DTOs
{
    public class LoanResponseSpruceExternal
    {
        [DeserializeAs(Name = "amount_Financed")]
        public decimal Amount_Financed { get; set; }
        [DeserializeAs(Name = "promo_Monthly_Payment")]
        public decimal Promo_Monthly_Payment { get; set; }
        [DeserializeAs(Name = "monthly_Payment_After_Promo_NO_QUALIFIED_PREPAYMENT")]
        public decimal Monthly_Payment_After_Promo_NO_QUALIFIED_PREPAYMENT { get; set; }
        [DeserializeAs(Name = "monthly_Payment_After_Promo_WITH_QUALIFIED_PREPAYMENT")]
        public decimal Monthly_Payment_After_Promo_WITH_QUALIFIED_PREPAYMENT { get; set; }
        [DeserializeAs(Name = "payment_Options")]
        public List<SprucePaymentOptionExternal> Payment_Options { get; set; }
        [DeserializeAs(Name = "calculator_Disclaimer")]
        public string Calculator_Disclaimer { get; set; }
        public List<SpruceErrorExternal> Calculator_Errors { get; set; }

        public class SprucePaymentOptionExternal
        {
            public int a_Term { get; set; }
            public decimal b_MonthlyPayment { get; set; }
        }

        public class SpruceErrorExternal
        {
            public string error { get; set; }
        }
    }
}
