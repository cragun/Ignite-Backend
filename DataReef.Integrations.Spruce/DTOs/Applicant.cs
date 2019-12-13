using System;

namespace DataReef.Integrations.Spruce.DTOs
{
    public class Applicant
    {
        public string NamePrefix { get; set; }

        public string NameSuffix { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public bool VerifyLicense { get; set; }

        public DateTime? BirthDate { get; set; }

        public string SSN { get; set; }

        public string InstallationAddress { get; set; }

        public string InstallationCity { get; set; }

        public string InstallationState { get; set; }

        public string InstallationZipCode { get; set; }

        public bool IsMailingDifferentInstall { get; set; }

        public string MailingAddressLine1 { get; set; }

        public string MailingAddressLine2 { get; set; }

        public string MailingCity { get; set; }

        public string MailingState { get; set; }

        public string MailingZipCode { get; set; }

        public string Email { get; set; }

        public string HomePhone { get; set; }

        public string CellPhone { get; set; }

        public bool HasCoApplicant { get; set; }
    }
}