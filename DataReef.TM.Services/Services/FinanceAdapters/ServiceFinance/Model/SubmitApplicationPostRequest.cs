using System;
using System.Runtime.Serialization;
using DataReef.Core.Extensions;
using DataReef.TM.Models.DTOs.FinanceAdapters;
using Newtonsoft.Json;

namespace DataReef.TM.Services.Services.FinanceAdapters.ServiceFinance.Model
{
    [DataContract]
    public class SubmitApplicationPostRequest
    {
        public SubmitApplicationPostRequest(SubmitApplicationRequest request)
        {
            TotalCost = request.TotalCost;
            DownPayment = request.DownPayment;
            FirstName = request.FirstName;
            MiddleInitial = request.MiddleInitial;
            LastName = request.LastName;
            DateOfBirth = request.DateOfBirth;
            Ssn = request.Ssn;
            Email = request.Email;
            PrimaryPhone = request.PrimaryPhone;
            CellPhone = request.CellPhone;
            WorkPhone = request.WorkPhone;
            EmployerName = request.EmployerName;
            GrossMonthlyIncome = request.GrossMonthlyIncome;
            JobYears = request.JobYears;
            JobMonths = request.JobMonths;
            MaritalStatus = (int)request.MaritalStatus;
            SpouseContactInformation = request.SpouseContactInformation;
            CurrentAddress = request.CurrentAddress;
            AptSuite = request.AptSuite;
            City = request.City;
            State = request.State;
            Zip = request.Zip;
            Residence = request.Residence.SplitCamelCase();
            MortgageRentalAmount = request.MortgageRentalAmount;
            YearsAtAddress = request.YearsAtAddress;
            MonthsAtAddress = request.MonthsAtAddress;
            PropertyType = request.PropertyType.SplitCamelCase();
            PropertyAddress = request.PropertyAddress;
            PropertyAptSuite = request.PropertyAptSuite;
            PropertyCity = request.PropertyCity;
            PropertyState = request.PropertyState;
            PropertyZip = request.PropertyZip;
            DlNumber = request.DlNumber;
            DlState = request.DlState;
            DlIssueDate = request.DlIssueDate;
            DlExpirationDate = request.DlExpirationDate;
            FirstNameCoApplicant = request.FirstNameCoApplicant;
            MiddleInitialCoApplicant = request.MiddleInitialCoApplicant;
            LastNameCoApplicant = request.LastNameCoApplicant;
            DateOfBirthCoApplicant = request.DateOfBirthCoApplicant;
            SsnCoApplicant = request.SsnCoApplicant;
            EmailCoApplicant = request.EmailCoApplicant;
            PrimaryPhoneCoApplicant = request.PrimaryPhoneCoApplicant;
            CellPhoneCoApplicant = request.CellPhoneCoApplicant;
            WorkPhoneCoApplicant = request.WorkPhoneCoApplicant;
            EmployerNameCoApplicant = request.EmployerNameCoApplicant;
            GrossMonthlyIncomeCoApplicant = request.GrossMonthlyIncomeCoApplicant;
            JobYearsCoApplicant = request.JobYearsCoApplicant;
            JobMonthsCoApplicant = request.JobMonthsCoApplicant;
            CurrentAddressCoApplicant = request.CurrentAddressCoApplicant;
            AptSuiteCoApplicant = request.AptSuiteCoApplicant;
            CityCoApplicant = request.CityCoApplicant;
            StateCoApplicant = request.StateCoApplicant;
            ZipCoApplicant = request.ZipCoApplicant;
            ResidenceCoApplicant = request.ResidenceCoApplicant.SplitCamelCase();
            MortgageRentalAmountCoApplicant = request.MortgageRentalAmountCoApplicant;
            YearsAtAddressCoApplicant = request.YearsAtAddressCoApplicant;
            MonthsAtAddressCoApplicant = request.MonthsAtAddressCoApplicant;
            DlNumberCoApplicant = request.DlNumberCoApplicant;
            DlStateCoApplicant = request.DlStateCoApplicant;
            DlIssueDateCoApplicant = request.DlIssueDateCoApplicant;
            DlExpirationDateCoApplicant = request.DlExpirationDateCoApplicant;
        }

        #region System

        [JsonProperty("source")]
        public string Source => "DATAREEF";

        [JsonProperty("ContactMethod")]
        public string ContactMethod => "WEB SERVICE";

        [JsonProperty("authorizedBy")]
        public string AuthorizedBy => "DEALER";

        [JsonProperty("ProgramType")]
        public string ProgramType { get; set; }

        [JsonProperty("Product")]
        public string Product => "SOLAR EQUIPMENT";

        #endregion

        #region Dealer

        [JsonProperty("Dealer.DealerNumber")]
        public string DealerNumber { get; set; }

        [JsonProperty("Dealer.Name")]
        public string DealerName { get; set; }

        [JsonProperty("Dealer.Amount")]
        public decimal TotalCost { get; set; }

        [JsonProperty("Dealer.Down")]
        public decimal DownPayment { get; set; }

        [JsonProperty("RequestedAmount")]
        public decimal AmountFinanced => TotalCost - DownPayment;

        #endregion

        #region Applicant

        [JsonProperty("PrimaryApplicant.FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("PrimaryApplicant.MiddleInitial")]
        public string MiddleInitial { get; set; }

        [JsonProperty("PrimaryApplicant.LastName")]
        public string LastName { get; set; }

        [JsonProperty("PrimaryApplicant.DateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("PrimaryApplicant.SSN")]
        public string Ssn { get; set; }

        [JsonProperty("PrimaryApplicant.EmailAddress")]
        public string Email { get; set; }

        [JsonProperty("PrimaryApplicant.HomePhone")]
        public string PrimaryPhone { get; set; }

        [JsonProperty("PrimaryApplicant.AlternatePhone")]
        public string CellPhone { get; set; }

        [JsonProperty("PrimaryApplicant.Employer.Phone")]
        public string WorkPhone { get; set; }

        [JsonProperty("PrimaryApplicant.Employer.Name")]
        public string EmployerName { get; set; }

        [JsonProperty("PrimaryApplicant.Employer.Income")]
        public decimal GrossMonthlyIncome { get; set; }

        [JsonProperty("PrimaryApplicant.Employer.JobYears")]
        public int JobYears { get; set; }

        [JsonProperty("PrimaryApplicant.Employer.JobMonths")]
        public int JobMonths { get; set; }

        // 1 if Married, 0 if Not
        [JsonProperty("PrimaryApplicant.Married")]
        public int MaritalStatus { get; set; }

        //  Email preferred or phone number
        [JsonProperty("PrimaryApplicant.SpouseContact")]
        public string SpouseContactInformation { get; set; }

        [JsonProperty("PrimaryApplicant.Address.StreetAddress")]
        public string CurrentAddress { get; set; }

        [JsonProperty("PrimaryApplicant.Address.StreetAddress2")]
        public string AptSuite { get; set; }

        [JsonProperty("PrimaryApplicant.Address.City")]
        public string City { get; set; }

        [JsonProperty("PrimaryApplicant.Address.State")]
        public string State { get; set; }

        [JsonProperty("PrimaryApplicant.Address.Zip")]
        public string Zip { get; set; }

        //  "Own" or "Rent"
        [JsonProperty("PrimaryApplicant.HousingType")]
        public string Residence { get; set; }

        [JsonProperty("PrimaryApplicant.MonthlyHousingPayment")]
        public decimal MortgageRentalAmount { get; set; }

        [JsonProperty("PrimaryApplicant.Address.AddressYears")]
        public int YearsAtAddress { get; set; }

        [JsonProperty("PrimaryApplicant.Address.AddressMonths")]
        public int MonthsAtAddress { get; set; }

        //  "Single Family" or "Manufactured Home"
        [JsonProperty("PrimaryApplicant.Property.Type")]
        public string PropertyType { get; set; }

        [JsonProperty("PrimaryApplicant.Property.Address.StreetAddress")]
        public string PropertyAddress { get; set; }

        [JsonProperty("PrimaryApplicant.Property.Address.StreetAddress2")]
        public string PropertyAptSuite { get; set; }

        [JsonProperty("PrimaryApplicant.Property.Address.City")]
        public string PropertyCity { get; set; }

        [JsonProperty("PrimaryApplicant.Property.Address.State")]
        public string PropertyState { get; set; }

        [JsonProperty("PrimaryApplicant.Property.Address.Zip")]
        public string PropertyZip { get; set; }

        [JsonProperty("PrimaryApplicant.OtherDocument")]
        public string OtherDocument => "DL";

        [JsonProperty("PrimaryApplicant.DriversLicenseNo")]
        public string DlNumber { get; set; }

        [JsonProperty("PrimaryApplicant.DriversLicenseState")]
        public string DlState { get; set; }

        [JsonProperty("PrimaryApplicant.DriversLicenseIssueDate")]
        public DateTime DlIssueDate { get; set; }

        [JsonProperty("PrimaryApplicant.DriversLicenseExpDate")]
        public DateTime DlExpirationDate { get; set; }

        #endregion

        #region CoApplicant

        [JsonProperty("CoApplicant.FirstName", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstNameCoApplicant { get; set; }

        [JsonProperty("CoApplicant.MiddleInitial", NullValueHandling = NullValueHandling.Ignore)]
        public string MiddleInitialCoApplicant { get; set; }

        [JsonProperty("CoApplicant.LastName", NullValueHandling = NullValueHandling.Ignore)]
        public string LastNameCoApplicant { get; set; }

        [JsonProperty("CoApplicant.DateOfBirth", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateOfBirthCoApplicant { get; set; }

        [JsonProperty("CoApplicant.SSN", NullValueHandling = NullValueHandling.Ignore)]
        public string SsnCoApplicant { get; set; }

        [JsonProperty("CoApplicant.EmailAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string EmailCoApplicant { get; set; }

        [JsonProperty("CoApplicant.HomePhone", NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryPhoneCoApplicant { get; set; }

        [JsonProperty("CoApplicant.AlternatePhone", NullValueHandling = NullValueHandling.Ignore)]
        public string CellPhoneCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Employer.Phone", NullValueHandling = NullValueHandling.Ignore)]
        public string WorkPhoneCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Employer.Name", NullValueHandling = NullValueHandling.Ignore)]
        public string EmployerNameCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Employer.Income", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? GrossMonthlyIncomeCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Employer.JobYears", NullValueHandling = NullValueHandling.Ignore)]
        public int? JobYearsCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Employer.JobMonths", NullValueHandling = NullValueHandling.Ignore)]
        public int? JobMonthsCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Address.StreetAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string CurrentAddressCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Address.StreetAddress2", NullValueHandling = NullValueHandling.Ignore)]
        public string AptSuiteCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Address.City", NullValueHandling = NullValueHandling.Ignore)]
        public string CityCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Address.State", NullValueHandling = NullValueHandling.Ignore)]
        public string StateCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Address.Zip", NullValueHandling = NullValueHandling.Ignore)]
        public string ZipCoApplicant { get; set; }

        //  "Own" or "Rent"
        [JsonProperty("CoApplicant.HousingType", NullValueHandling = NullValueHandling.Ignore)]
        public string ResidenceCoApplicant { get; set; }

        [JsonProperty("CoApplicant.MonthlyHousingPayment", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? MortgageRentalAmountCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Address.AddressYears", NullValueHandling = NullValueHandling.Ignore)]
        public int? YearsAtAddressCoApplicant { get; set; }

        [JsonProperty("CoApplicant.Address.AddressMonths", NullValueHandling = NullValueHandling.Ignore)]
        public int? MonthsAtAddressCoApplicant { get; set; }

        [JsonProperty("CoApplicant.OtherDocument", NullValueHandling = NullValueHandling.Ignore)]
        public string OtherDocumentCoApplicant => "DL";

        [JsonProperty("CoApplicant.DriversLicenseNo", NullValueHandling = NullValueHandling.Ignore)]
        public string DlNumberCoApplicant { get; set; }

        [JsonProperty("CoApplicant.DriversLicenseState", NullValueHandling = NullValueHandling.Ignore)]
        public string DlStateCoApplicant { get; set; }

        [JsonProperty("CoApplicant.DriversLicenseIssueDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DlIssueDateCoApplicant { get; set; }

        [JsonProperty("CoApplicant.DriversLicenseExpDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DlExpirationDateCoApplicant { get; set; }

        #endregion
    }
}
