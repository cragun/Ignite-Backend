namespace DataReef.TM.Models.DataViews.Financing.PaymentFactors
{
    public class PaymentFactorsPlanData
    {
        /// <summary>
        /// Number of months for 1st period
        /// </summary>
        public int FirstPeriodMonths { get; set; }

        /// <summary>
        /// Federal tax incentives applies in this month.
        /// </summary>
        public int ITCMonth { get; set; }

        /// <summary>
        /// Factor used for 1st period
        /// </summary>
        public double Factor01 { get; set; }

        /// <summary>
        /// Factor used for 2nd period when 30% is applied to Principal
        /// </summary>
        public double Factor02 { get; set; }

        /// <summary>
        /// Factor used for 2nd period when 30% is not applied to Principal.
        /// </summary>
        public double? Factor03 { get; set; }

        public decimal GetMonthlyPayment(int monthIndex, decimal amount, bool applyITC = true)
        {
            // if factor03 has value, and ITC does not get applied, use Factor03
            var factor2 = Factor03.HasValue && !applyITC ? Factor03.Value : Factor02;
            var paymentFactor = (decimal)((monthIndex <= FirstPeriodMonths ? Factor01 : factor2) / 100);
            return (paymentFactor * amount);
        }

        public bool IsValid()
        {
            return FirstPeriodMonths > 0
                 && Factor01 > 0
                 && Factor02 > 0;
        }
    }
}
