
using DataReef.TM.Models.DTOs.Solar;
using System.Collections.Generic;
namespace DataReef.Integrations.Spruce.DTOs
{
    public class LoanRequestSpruce
    {
        /// <summary>
        /// Unique identifier from the client to identify scenarios
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The possible values are INTONLY, SAC, SF
        /// </summary>
        public string RateType { get; set; }

        /// <summary>
        /// Number of days that interest is deferred. Kilowatt will provide this information for your specific products.
        /// </summary>
        public int DaysDeferred { get; set; }

        /// <summary>
        /// Duration of the scenario in years (including all the years of the financing period)
        /// </summary>
        public int ScenarioTerm { get; set; }

        /// <summary>
        /// Duration of the financing period in years.
        /// </summary>
        public int Term { get; set; }

        /// <summary>
        /// Intro term duration in months for a product such as finance charge only. Kilowatt will provide this information for your specific products.
        /// </summary>
        public int IntroTerm { get; set; }

        /// <summary>
        /// $/W
        /// </summary>
        public decimal PricePerWattASP { get; set; }

        public long SystemSize { get; set; }

        public decimal TaxRate { get; set; }

        public decimal DownPayment { get; set; }

        /// <summary>
        /// First year Production in kWh
        /// </summary>
        public int FirstYearAnnualProduction { get; set; }

        /// <summary>
        /// $/kWh
        /// </summary>
        public decimal UtilityElectricityCost { get; set; }

        /// <summary>
        /// Percentge
        /// </summary>
        public double UtilityInflationRate { get; set; }

        /// <summary>
        /// annual system degredation rate
        /// </summary>
        public double Derate { get; set; }


        public List<MonthlyPower> Consumption { get; set; }

        /// <summary>
        /// Expected APR (Average Percentage Rate), such as 4.99
        /// </summary>
        public decimal Apr { get; set; }

        /// <summary>
        /// The ppercentage of the AmountFinanced that represents the federal tax incentive
        /// </summary>
        public decimal FederalTaxIncentivePercentage { get; set; }

        /// <summary>
        /// The amount of money that are being received from the states that offer this incentive
        /// </summary>
        public decimal StateTaxIncentive { get; set; }

        /// <summary>
        /// If specifying True, The API will return other financing terms and monthly payments to be used in comparison.
        /// </summary>
        public bool AddPaymentOptions { get; set; }


        /// <summary>
        /// If true, the months will be included in the response
        /// </summary>
        public bool IncludeMonthsInResponse { get; set; }
    }
}
