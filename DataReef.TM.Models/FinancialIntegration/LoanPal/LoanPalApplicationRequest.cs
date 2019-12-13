using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.FinancialIntegration.LoanPal
{
    public class LoanPalApplicationRequest
    {
        //public string Id { get; set; }

        public string ReferenceNumber { get; set; }

        public LoanPalApplicantModel Applicant { get; set; }

        public List<LoanPalApplicantModel> CoApplicants { get; set; }

        public LoanPalSalesRepModel SalesRep { get; set; }

        public double TotalSystemCost { get; set; }

        public bool BatteryIncluded { get; set; }

        public int SelectedLoanOption { get; set; }

        public string Source { get; set; }
    }
}
