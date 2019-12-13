using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.FinancialIntegration.LoanPal
{
    public class LoanPalApplicationResponse
    {
        public string LoanId { get; set; }

        public List<LoanPalLoanOptionModel> LoanOptions { get; set; }

        public string MaxLoanAmount { get; set; }

        public string Reason { get; set; }

        public string ReferenceNumber { get; set; }

        public string Status { get; set; }

        public List<LoanPalStipulationModel> Stipulations { get; set; }

        public string TotalSystemCost { get; set; }

        public List<LoanPalErrorModel> Errors { get; set; }

        public string Message { get; set; }
    }
}
