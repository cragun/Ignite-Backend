using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using DataReef.Core.Attributes.Validation;

namespace DataReef.TM.Models.DTOs.FinanceAdapters
{
    [DataContract]
    public class SubmitApplicationRequest
    {
        [DataMember]
        [ValidGuid]
        public Guid PlanDefinitionID { get; set; }

        [DataMember]
        [ValidGuid]
        public Guid OUID { get; set; }

        #region Dealer

        [DataMember]
        [Range(1, int.MaxValue, ErrorMessage = "TotalCost must be greater that 1")]
        public decimal TotalCost { get; set; }

        [DataMember]
        public decimal DownPayment { get; set; }

        [IgnoreDataMember]
        public decimal AmountFinanced => TotalCost - DownPayment;

        #endregion

        #region Applicant

        [DataMember]
        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [DataMember]
        [StringLength(1)]
        public string MiddleInitial { get; set; }

        [DataMember]
        [Required]
        [StringLength(80)]
        public string LastName { get; set; }

        [DataMember]
        [Required]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        [Required]
        public string Ssn { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        [Required]
        public string PrimaryPhone { get; set; }

        [DataMember]
        public string CellPhone { get; set; }

        [DataMember]
        public string WorkPhone { get; set; }

        [DataMember]
        [Required]
        [StringLength(80)]
        public string EmployerName { get; set; }

        [DataMember]
        [Range(1, double.MaxValue, ErrorMessage = "GrossMonthlyIncome must be greater than 1")]
        public decimal GrossMonthlyIncome { get; set; }

        [DataMember]
        public int JobYears { get; set; }

        [DataMember]
        [Range(0, 12, ErrorMessage = "JobMonths must be between 0 and 12")]
        public int JobMonths { get; set; }

        // 1 if Married, 0 if Not
        [DataMember]

        public MaritalStatus MaritalStatus { get; set; }

        //  Email preferred or phone number
        [DataMember]
        public string SpouseContactInformation { get; set; }

        [DataMember]
        [Required]
        [StringLength(40)]
        public string CurrentAddress { get; set; }

        [DataMember]        
        [StringLength(40)]
        public string AptSuite { get; set; }

        [DataMember]
        [Required]
        [StringLength(25)]
        public string City { get; set; }

        [DataMember]
        [Required]
        [StringLength(2)]
        public string State { get; set; }

        [DataMember]
        [Required]
        [StringLength(5)]
        public string Zip { get; set; }

        //  "Own" or "Rent"
        [DataMember]
        public Residence Residence { get; set; }

        [DataMember]
        [Required]
        public decimal MortgageRentalAmount { get; set; }

        [DataMember]
        public int YearsAtAddress { get; set; }

        [DataMember]
        [Range(0, 12, ErrorMessage = "MonthsAtAddress must be between 0 and 12")]
        public int MonthsAtAddress { get; set; }

        //  "Single Family" or "Manufactured Home"
        [DataMember]
        public PropertyType PropertyType { get; set; }

        [DataMember]
        [StringLength(40)]
        public string PropertyAddress { get; set; }

        [DataMember]
        [StringLength(40)]
        public string PropertyAptSuite { get; set; }

        [DataMember]
        [Required]
        [StringLength(25)]
        public string PropertyCity { get; set; }

        [DataMember]
        [Required]
        [StringLength(2)]
        public string PropertyState { get; set; }

        [DataMember]
        [Required]
        [StringLength(5)]
        public string PropertyZip { get; set; }

        [DataMember]
        [Required]
        [StringLength(50)]
        public string DlNumber { get; set; }

        [DataMember]
        [Required]
        [StringLength(2)]
        public string DlState { get; set; }

        [DataMember]
        public DateTime DlIssueDate { get; set; }

        [DataMember]
        public DateTime DlExpirationDate { get; set; }

        #endregion

        #region CoApplicant

        [DataMember]
        [StringLength(20)]
        public string FirstNameCoApplicant { get; set; }

        [DataMember]
        [StringLength(1)]
        public string MiddleInitialCoApplicant { get; set; }

        [DataMember]
        [StringLength(80)]
        public string LastNameCoApplicant { get; set; }

        [DataMember]
        public DateTime? DateOfBirthCoApplicant { get; set; }

        [DataMember]
        public string SsnCoApplicant { get; set; }

        [DataMember]
        public string EmailCoApplicant { get; set; }

        [DataMember]
        public string PrimaryPhoneCoApplicant { get; set; }

        [DataMember]
        public string CellPhoneCoApplicant { get; set; }

        [DataMember]
        public string WorkPhoneCoApplicant { get; set; }

        [DataMember]
        [StringLength(80)]
        public string EmployerNameCoApplicant { get; set; }

        [DataMember]
        public decimal? GrossMonthlyIncomeCoApplicant { get; set; }

        [DataMember]
        public int? JobYearsCoApplicant { get; set; }

        [DataMember]
        [Range(0, 12, ErrorMessage = "JobMonthsCoApplicant must be between 0 and 12")]
        public int? JobMonthsCoApplicant { get; set; }

        [DataMember]
        [StringLength(40)]
        public string CurrentAddressCoApplicant { get; set; }

        [DataMember]
        [StringLength(40)]
        public string AptSuiteCoApplicant { get; set; }

        [DataMember]
        [StringLength(25)]
        public string CityCoApplicant { get; set; }

        [DataMember]
        [StringLength(2)]
        public string StateCoApplicant { get; set; }

        [DataMember]
        [StringLength(5)]
        public string ZipCoApplicant { get; set; }

        //  "Own" or "Rent"
        [DataMember]
        public Residence? ResidenceCoApplicant { get; set; }

        [DataMember]
        public decimal? MortgageRentalAmountCoApplicant { get; set; }

        [DataMember]
        public int? YearsAtAddressCoApplicant { get; set; }

        [DataMember]
        [Range(0, 12, ErrorMessage = "MonthsAtAddressCoApplicant must be between 0 and 12")]
        public int? MonthsAtAddressCoApplicant { get; set; }

        [DataMember]
        [StringLength(50)]
        public string DlNumberCoApplicant { get; set; }

        [DataMember]
        [StringLength(2)]
        public string DlStateCoApplicant { get; set; }

        [DataMember]
        public DateTime? DlIssueDateCoApplicant { get; set; }

        [DataMember]
        public DateTime? DlExpirationDateCoApplicant { get; set; }

        #endregion
    }
}
