using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class LoanResponse
    {
        public List<PaymentMonth> Months { get; set; }

        public List<PaymentYear> Years { get; set; }

        /// <summary>
        /// Total amount that will be financed.
        /// </summary>
        public decimal AmountFinanced { get; set; }

        /// <summary>
        /// Purchase prise / Gross System Cost
        /// </summary>
        public decimal SolarSystemCost { get; set; }

        public decimal TotalAddersCosts { get; set; }

        public decimal AddersPaidByRep { get; set; }

        //  The total incentives (of the ones based on the adders model
        public decimal TotalIncentives { get; set; }

        /// <summary>
        /// levelized cost of energy.  What you pay for the energy you take off the system ( only that energy ).  Includes costs, benefits
        /// </summary>
        public decimal Lcoe => Math.Round(GetLCOE(), 2);

        /// <summary>
        /// the apr of the main loan
        /// </summary>
        public decimal StatedApr { get; set; }

        /// <summary>
        /// the blended apr of all financing across all loans
        /// </summary>
        public decimal ActualApr { get; set; }

        /// <summary>
        /// The number of months without payment at the beginning of the contract
        /// </summary>
        public int DeferredPeriodInMonths { get; set; }

        public int IntroPeriodInMonths { get; set; }

        /// <summary>
        /// How many months for the 1st payment period when doing Payment Factors
        /// </summary>
        public int PaymentFactorsFirstPeriod { get; set; } = 18;

        /// <summary>
        /// Number of months of financing, including deferred months if they exist
        /// </summary>
        public int MainLoanPeriodInMonths { get; set; }

        [JsonIgnore]
        public int FinancingPeriodInMonths => IntroPeriodInMonths + MainLoanPeriodInMonths;


        /// <summary>
        /// Monthly payment amount during the promotion period.
        /// </summary>
        public decimal IntroMonthlyPayment { get; set; }

        public decimal MonthlyPayment { get; set; }
        public decimal MonthlyPaymentNoITC { get; set; }

        public decimal TotalInterestPayment { get; set; }

        #region BenefitsAndIncentives

        /// <summary>
        /// The amount of federal money that are being received 
        /// </summary>
        public decimal TotalFederalTaxIncentive { get; set; }

        /// <summary>
        /// The amount of money that are being received from the states that offer this incentive
        /// </summary>
        public decimal TotalStateTaxIncentive { get; set; }

        /// <summary>
        /// Performance-based incentives
        /// </summary>
        public decimal TotalPBI { get; set; }

        /// <summary>
        /// Solar Renewable Energy Credit
        /// </summary>
        public decimal TotalSREC { get; set; }

        public decimal TotalUpfrontRebate { get; set; }

        public decimal TotalUpfrontRebateReducedFromITC { get; set; }

        public decimal TotalBenefitsAndIncentives { get; set; }

        public decimal TotalTakeHomeIncentives { get; set; }

        #endregion BenefitsAndIncentives

        #region solar system data

        public long TotalSystemProduction { get; set; }
        public double TotalConsumption { get; set; }
        public double PostAddersConsumption { get; set; }

        public decimal TotalElectricityBillWithoutSolar { get; set; }
        public decimal TotalElectricityBillWithSolar { get; set; }


        public decimal FirstYearMonthlyElectricityBillWithSolar { get; set; }
        public decimal FirstYearMonthlyElectricityBillWithoutSolar { get; set; }

        public decimal AvgMonthlyElectrityBillWithSolar { get; set; }
        public decimal AvgMonthlyElectrityBillWithoutSolar { get; set; }


        public decimal TotalSolarPaymentsCost { get; set; }
        public decimal TotalSavings { get; set; }

        #endregion

        public decimal GetLCOE()
        {
            return (TotalSolarPaymentsCost - TotalBenefitsAndIncentives) / Math.Max(TotalSystemProduction, 1);
        }
    }
}
