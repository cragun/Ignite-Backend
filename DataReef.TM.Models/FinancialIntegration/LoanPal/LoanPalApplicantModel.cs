using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.FinancialIntegration.LoanPal
{
    public class LoanPalApplicantModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string SSN { get; set; }

        public double AnnualIncome { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool ElectronicConsent { get; set; }

        public string Employer { get; set; }

        public string Occupation { get; set; }

        public string SpokenLanguage { get; set; }

        public LoanPalAddressModel Address { get; set; }
    }
}
