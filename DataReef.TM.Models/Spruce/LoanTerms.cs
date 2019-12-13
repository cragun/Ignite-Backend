using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Spruce
{
    public class LoanTerms
    {
        public decimal AmountFinanced   { get; set; }

        public int GotoTerm             { get; set; }

        public decimal IntroRatePayment { get; set; }

        public int IntroTerm            { get; set; }

        public decimal LoanRate         { get; set; }

        public decimal MaxApproved      { get; set; }

        public decimal MonthlyPayment   { get; set; }

        public int Term                 { get; set; }
    }
}
