using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    public class SavingAnalysisRequestModel
    {
        public string ProviderAccountId             { get; set; }

        public decimal? SolarRateAmount             { get; set; }

        public string ElectricityProviderProfileId  { get; set; }

        public List<string> SolarProviderProfileIDs { get; set; }

        public string RateInflation                 { get; set; }

        public DateTime? FromDate                   { get; set; }

        public bool GenerateSlope                   { get; set; }

        public double AnnualOutputDegradation       { get; set; }

    }
}