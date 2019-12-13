using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Financing.SalalCreditUnion
{
    public class SalalCreditUnionFinancingPlanData
    {
        public int NiNpMonths { get; set; }
        public decimal PaymentFactor { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }

        public decimal GetMonthlyPayment(int monthIndex, decimal amount)
        {
            if (monthIndex <= NiNpMonths)
            {
                return 0;
            }
            return PaymentFactor * amount;
        }
    }
}
