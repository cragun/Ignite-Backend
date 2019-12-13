using DataReef.TM.Models.DTOs.Solar.Finance;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    public class ProposalForecast
    {
        public List<ForecastMonth> Months { get; set; }
        public List<ForecastYear> Years { get; set; }

        public decimal TotalCost { get; set; }
        public decimal FedTaxCredit { get; set; }
        public Dictionary<string, decimal> StdRebates { get; set; }
        public Dictionary<string, decimal> SmartRebates { get; set; }
        public Dictionary<string, decimal> SmarterRebates { get; set; }
        public decimal NetCost { get; set; }

        public ProposalForecast()
        { }

        public ProposalForecast(LoanResponse forecastData)
        {
            Months = forecastData?
                        .Months?
                        .Select(m => new ForecastMonth
                        {
                            Year = m.Year,
                            Month = m.Month,
                            Payment = m.PaymentAmount,
                            FederalTaxIncentive = m.FederalTaxIncentive,
                            StateTaxIncentive = m.StateTaxIncentive,
                            TotalBenefitsAndIncentives = m.TotalBenefitsAndIncentives,
                            SystemProduction = m.SystemProduction,
                            Consumption = m.Consumption,
                            ElectricityBillWithoutSolar = m.ElectricityBillWithoutSolar,
                            ElectricityBillWithSolar = m.ElectricityBillWithSolar,
                            Savings = m.Savings,
                        })
                        .ToList();

            Years = forecastData?
                        .Years?
                        .Select(m => new ForecastYear
                        {
                            Year = m.Year,
                            Payment = m.PaymentAmount,
                            FederalTaxIncentive = m.FederalTaxIncentive,
                            StateTaxIncentive = m.StateTaxIncentive,
                            TotalBenefitsAndIncentives = m.TotalBenefitsAndIncentives,
                            SystemProduction = m.SystemProduction,
                            Consumption = m.Consumption,
                            ElectricityBillWithoutSolar = m.ElectricityBillWithoutSolar,
                            ElectricityBillWithSolar = m.ElectricityBillWithSolar,
                            Savings = m.Savings
                        })
                        .ToList();
        }
    }

    public class ForecastMonth : ForecastBase
    {
        public int Month { get; set; }
    }

    public class ForecastYear : ForecastBase
    {
    }


    public abstract class ForecastBase
    {
        public int Year { get; set; }


        public decimal Payment { get; set; }

        //public decimal Interest { get; set; }

        //public decimal Principal { get; set; }

        //public decimal Balance { get; set; }

        public decimal FederalTaxIncentive { get; set; }

        /// <summary>
        /// The amount of money that are being received from the states that offer this incentive
        /// </summary>
        public decimal StateTaxIncentive { get; set; }

        ///// <summary>
        ///// Performance-based incentives
        ///// </summary>
        //public decimal PBI { get; set; }

        ///// <summary>
        ///// Solar Renewable Energy Credit
        ///// </summary>
        //public decimal SREC { get; set; }
        //public decimal TakeHomeIncentives { get; set; }

        public decimal TotalBenefitsAndIncentives;

        public double SystemProduction { get; set; }

        public double Consumption { get; set; }

        public decimal ElectricityBillWithoutSolar { get; set; }

        public decimal ElectricityBillWithSolar { get; set; }

        public decimal Savings { get; set; }
    }
}
