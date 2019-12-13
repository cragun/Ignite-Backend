using DataReef.TM.Models.DTOs.Solar.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Engines.FinancialEngine.Models
{
    public class LoanAmortizationData
    {
        public List<PaymentMonth> Months { get; set; }
        public double MainLoanApr { get; set; }
        public int DeferredPeriodInMonths { get; set; }
        public int IntroPeriodInMonths { get; set; }
        public int MainLoanPeriodInMonths { get; set; }
        public decimal IntroMonthlyPayment { get; set; }
        public decimal MainLoanMonthlyPayment { get; set; }
        public decimal MainLoanMonthlyPaymentNoITC { get; set; }

        public int PaymentFactorsFirstPeriod { get; set; } = 18;

    }
}
