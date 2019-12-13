using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Spruce
{
    [Table("Applicants", Schema = "Spruce")]
    public class Applicant : EntityBase
    {
        [DataMember]
        public string NamePrefix { get; set; }

        [DataMember]
        public string NameSuffix { get; set; }

        [DataMember]
        [MaxLength(24)]
        public string FirstName { get; set; }

        [DataMember]
        [MaxLength(15)]
        public string MiddleName { get; set; }

        [DataMember]
        [MaxLength(24)]
        public string LastName { get; set; }

        [DataMember]
        public bool VerifyLicense { get; set; }

        [DataMember]
        public DateTime? BirthDate { get; set; }

        [DataMember]
        [MaxLength(9)]
        public string SSN { get; set; }

        [DataMember]
        [MaxLength(35)]
        public string InstallationAddress { get; set; }

        [DataMember]
        [MaxLength(25)]
        public string InstallationCity { get; set; }

        [DataMember]
        [MaxLength(2)]
        public string InstallationState { get; set; }

        [DataMember]
        [MaxLength(5)]
        public string InstallationZipCode { get; set; }

        [DataMember]
        public bool IsMailingDifferentInstall { get; set; }

        [DataMember]
        [MaxLength(35)]
        public string MailingAddressLine1 { get; set; }

        [DataMember]
        [MaxLength(35)]
        public string MailingAddressLine2 { get; set; }

        [DataMember]
        [MaxLength(25)]
        public string MailingCity { get; set; }

        [DataMember]
        [MaxLength(2)]
        public string MailingState { get; set; }

        [DataMember]
        [MaxLength(5)]
        public string MailingZipCode { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string HomePhone { get; set; }

        [DataMember]
        public string CellPhone { get; set; }

        [DataMember]
        public bool HasCoApplicant { get; set; }

        #region Navigation

        [DataMember]
        [Required]
        public QuoteRequest QuoteRequest { get; set; }

        #endregion

        public bool IsValid(int methodID)
        {
            switch (methodID)
            {
                case 1:
                    return BirthDate.HasValue
                        && (!string.IsNullOrWhiteSpace(HomePhone) || !string.IsNullOrWhiteSpace(CellPhone))
                        && !string.IsNullOrWhiteSpace(SSN)
                        && (!HasCoApplicant || (HasCoApplicant
                                                && QuoteRequest.CoAppInfo != null
                                                && QuoteRequest.CoAppEmployment != null
                                                && QuoteRequest.CoAppIncomeDebt != null
                                                && QuoteRequest.CoAppInfo.IsValid(methodID)));
                case 2:
                    return !HasCoApplicant || (HasCoApplicant
                                                && QuoteRequest.CoAppInfo != null
                                                && QuoteRequest.CoAppInfo.IsValid(methodID)); ;
            }
            return false;
        }
    }
}
