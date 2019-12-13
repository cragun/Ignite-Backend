using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Financing.Sunnova
{
    public class SunnovaFinancingPlanData
    {
        public static readonly int FirstPeriodNumberOfMonths = 18;

        public double PaymentFactor_Month1_18 { get; set; }
        public double PaymentFactor_Month19_end { get; set; }

        public decimal GetMonthlyPayment(int monthIndex, decimal amount)
        {
            var paymentFactor = (decimal)((monthIndex <= FirstPeriodNumberOfMonths ? PaymentFactor_Month1_18 : PaymentFactor_Month19_end) / 100);
            return (decimal)(paymentFactor * amount);
        }
    }
}
