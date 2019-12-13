using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Commerce
{
    public class LeadOrderDetailDto
    {
        public Address Address { get; set; }

        public Person OriginalPerson { get; set; }

        public int FinancialScore { get; set; }

        public double? SolarScore { get; set; }

        public Property Property { get; set; }

        public Family Family { get; set; }

        public Mortgage CurrentMortgage { get; set; }

        public Avm Avm { get; set; }

        public Roof Roof { get; set; }

        public Prescreen Prescreen { get; set; }

        public LeadOrderDetailDto_CSV ForCsv()
        {
            return new LeadOrderDetailDto_CSV
            {
                Name = OriginalPerson?.Name,
                Address = Address?.FullAddress,
                City = Address?.City,
                State = Address?.State,
                ZipCode = Address?.ZipCode,

                FinancingAvailable = (Prescreen != null && !string.IsNullOrWhiteSpace( Prescreen?.Bucket))  ? "YES":"NO",
                FinancingScore = FinancialScore,
                SolarScore = SolarScore,

                SquareFeet = Property?.SquareFeet,
                YearBuilt = Property?.YearBuilt,
                BedroomCount = Property?.BedroomCount,
                ExteriorType = Property?.ExteriorType,
                CoolingType = Property?.CoolingType,
                EstimatedIncome = Family?.EstimatedIncome,
                NumberOfAdults= Family?.NumberOfAdults,
                NumberOfChildren = Family?.NumberOfChildren,
                YearsInHome = Family?.YearsInHome,
                PhoneNumber = Family?.PhoneNumber,
                MortgageDate = CurrentMortgage?.RecordingDate?.Year >=1800? CurrentMortgage?.RecordingDate : null,
                MortgageOriginalBalance = CurrentMortgage?.OriginalBalance,
                MortageCurrentBalance = CurrentMortgage?.CurrentBalance,
                MortgageMonthlyPayment = CurrentMortgage?.MonthlyPayment,
                MortgageYearsRemaining = CurrentMortgage?.YearsRemaining>0? CurrentMortgage?.YearsRemaining:0,
                NewConstructionFlag = CurrentMortgage?.NewConstructionFlag,
                RefiFlag = CurrentMortgage?.RefiFlag,
                HelocFlag = CurrentMortgage?.HelocFlag,
                MortageTerm = CurrentMortgage?.Term / 12,
                Avm = Avm?.AvmValue,

                RoofScore = Roof?.RoofRating,
                Azimuth = Roof?.HouseHeading,

                Lifestyles= Family?.Lifestyles!=null? String.Join("; ", Family?.Lifestyles?.ToArray()):""
            };
        }
    }

    public class LeadOrderDetailDto_CSV
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public string FinancingAvailable { get; set; }

        public int FinancingScore { get; set; }

        public double? SolarScore { get; set; }

        public int? RoofScore { get; set; }

        public int? Azimuth { get; set; }

        public int? SquareFeet { get; set; }
        public int? YearBuilt { get; set; }
        public int? BedroomCount { get; set; }
        public string ExteriorType { get; set; }
        public string HeatingType { get; set; }
        public string CoolingType { get; set; }

        public int? EstimatedIncome { get; set; }
        public int? NumberOfAdults { get; set; }
        public int? NumberOfChildren { get; set; }
        public int? YearsInHome { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime? MortgageDate { get; set; }
        public double? MortgageOriginalBalance { get; set; }
        public double? MortageCurrentBalance { get; set; }
        public double? MortgageMonthlyPayment { get; set; }
        public int? MortgageYearsRemaining { get; set; }
        public string NewConstructionFlag { get; set; }
        public string RefiFlag { get; set; }
        public string HelocFlag { get; set; }
        public int? MortageTerm { get; set; }
      
        public double? Avm { get; set; }

        public string Lifestyles { get; set; }


    }

    public class Address
    {
        [JsonProperty("Address")]
        public string FullAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
            set { }
        }
    }

    public class Property
    {
        public int SquareFeet { get; set; }
        public int NumberOfFloors { get; set; }
        public int YearBuilt { get; set; }
        public string ConstructionType { get; set; }
        public int LotSize { get; set; }
        public int BedroomCount { get; set; }
        public double BathroomCount { get; set; }
        public string ExteriorType { get; set; }
        public string HeatingType { get; set; }
        public string CoolingType { get; set; }
        public double EstimatedHomeValue { get; set; }
        public double AVM { get; set; }
       
    }

    public class Family
    {
        public int EstimatedIncome { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int YearsInHome { get; set; }
        public IList<string> Lifestyles { get; set; }
        public string PhoneNumber { get; set; }
        public string Guid { get; set; }
        public DateTime? DateCreated { get; set; }
    }


    public class Mortgage
    {
        public string BuyerName1 { get; set; }
        public string BuyerName2 { get; set; }
        public string SellerName { get; set; }
        public DateTime? SaleDate { get; set; }
        public DateTime? RecordingDate { get; set; }
        public DateTime? FirstMortgageRecordingDate { get; set; }
        public double Apr { get; set; }
        public double OriginalBalance { get; set; }
        public double CurrentBalance { get; set; }
        public double MonthlyPayment { get; set; }
        public int YearsRemaining { get; set; }
        public string NewConstructionFlag { get; set; }
        public string RefiFlag { get; set; }
        public string HelocFlag { get; set; }
        public double InterestPaid { get; set; }
        public double InterestRemaining { get; set; }
        public int InterestRateType { get; set; }
        public int Term { get; set; }
        public string ForclosureFlag { get; set; }
        public string TitleCompany { get; set; }
        public string SubdivisionName { get; set; }
        public object Apn { get; set; }
        public string TransactionNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string Guid { get; set; }
        public DateTime? DateCreated { get; set; }
    }

    public class Roof
    {
        public int RoofRating { get; set; }
        public int HouseHeading { get; set; }
       
    }


    public class Avm
    {
        [JsonProperty("Avm")]
        public double AvmValue { get; set; }
     
    }


    public class Prescreen
    {
        public string Name { get; set; }
        public int Bureau { get; set; }
        public string Bucket { get; set; }
        public bool DidQualify { get; set; }
        public DateTime? DateProcessed { get; set; }
        public string Guid { get; set; }
        public DateTime? DateCreated { get; set; }
    }





}
