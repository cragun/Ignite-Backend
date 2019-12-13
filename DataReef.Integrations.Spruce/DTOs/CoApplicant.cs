using System;

namespace DataReef.Integrations.Spruce.DTOs
{
    public class CoApplicant
    {
        public string NamePrefix { get; set; }

        public string NameSuffix { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public bool VerifyLicense { get; set; }

        public DateTime? BirthDate { get; set; }

        public string SSN { get; set; }

        public bool IsCoAppDifferentMailing { get; set; }

        public string MailingAddressLine1 { get; set; }

        public string MailingAddressLine2 { get; set; }

        public string MailingCity { get; set; }

        public string MailingState { get; set; }

        public string MailingZipCode { get; set; }

        public string Email { get; set; }

        public string HomePhone { get; set; }

        public string CellPhone { get; set; }
    }
}