using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.LoanPal.Models.LoanPal
{
    public class ApplicantModel
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("dob")]
        public string DateOfBirth { get; set; }

        [JsonProperty("ssn")]
        public string SSN { get; set; }

        [JsonProperty("annualIncome")]
        public string AnnualIncome { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("electronicConsent")]
        public bool ElectronicConsent { get; set; }

        [JsonProperty("employer")]
        public string Employer { get; set; }

        [JsonProperty("occupation")]
        public string Occupation { get; set; }

        [JsonProperty("spokenLanguage")]
        public string SpokenLanguage { get; set; }

        [JsonProperty("address")]
        public AddressModel Address { get; set; }

    }
}
