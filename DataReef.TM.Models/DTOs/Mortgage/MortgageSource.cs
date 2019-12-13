using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataReef.TM.Models.DTOs.Mortgage
{
    //  TODO: add descriptions for fields
    public class MortgageSource
    {
        [JsonProperty("Buyer1BuyerName")]
        public string Buyer1Name { get; set; }

        [JsonProperty("Buyer2BuyerName")]
        public string Buyer2Name { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public string HouseNumber { get; set; }

        public string StreetName { get; set; }

        public string RecordingDate { get; set; }

        public string FirstMortgageRecordingDate { get; set; }

        public decimal FirstMortgageAmount { get; set; }

        public decimal FirstMortgageEstimatedInterestRate { get; set; }

        public string FirstMortgageTermCode { get; set; }

        public int? FirstMortgageTerm { get; set; }

        public string FirstMortgageDueDate { get; set; }

        public bool NewConstructionFlag { get; set; }

        public bool ReoSaleFlag { get; set; }

        public string FirstMortgageEquityFlag { get; set; }

        public string FirstMortgageHelocFlag { get; set; }

        public int? FirstMortgageLoanTypeCode { get; set; }

        public decimal Ave { get; set; }

        public decimal CurrentMortgageBalance { get; set; }

        public decimal LoanToValue { get; set; }

        public ICollection<string> Exceptions { get; set; }
    }
}
