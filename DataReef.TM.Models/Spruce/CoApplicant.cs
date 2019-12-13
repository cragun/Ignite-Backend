using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Spruce
{
    [Table("CoApplicants", Schema = "Spruce")]
    public class CoApplicant : EntityBase
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
        public bool IsCoAppDifferentMailing { get; set; }

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
                        && !string.IsNullOrWhiteSpace(SSN);                        
            }
            return true;
        }
    }
}
