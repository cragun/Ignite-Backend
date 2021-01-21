using DataReef.TM.Models.DTOs.Solar.Finance.Enums;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class FinanceEstimate
    {
        public FinanceEstimate() { }

        public FinanceEstimate(LoanResponse response, FinancePlanDefinition plan, decimal mortgagePayment, LoanRequest request = null)
        {
            FinancePlanDefinitionId = plan.Guid; 
            CanBuild = plan.Type == FinancePlanType.Cash || plan.Type == FinancePlanType.Loan || plan.Type == FinancePlanType.Lease;
            FinanceType = plan.Type;

            FirstYearMonthlyElectricityCosts = response.FirstYearMonthlyElectricityBillWithSolar;
            AverageMonthlyElectricityCosts = response.AvgMonthlyElectrityBillWithSolar;

            LoanPayment = plan.Type != FinancePlanType.Mortgage ? response.MonthlyPayment : 0;
            InterestRate = response.StatedApr;
            MortgagePayment = mortgagePayment;
            switch (plan.Type)
            {
                case FinancePlanType.PPA:
                case FinancePlanType.Pace:
                case FinancePlanType.ReverseMortgage:
                case FinancePlanType.Mortgage:
                case FinancePlanType.Loan:
                    Terms = (response.MainLoanPeriodInMonths + response.IntroPeriodInMonths + response.DeferredPeriodInMonths) / 12;
                    break;
                case FinancePlanType.None:
                case FinancePlanType.Cash:
                    Terms = 0;
                    break;
                case FinancePlanType.Lease:
                    Terms = request?.ScenarioTermInYears ?? 0;
                    break;
            }

            Lcoe = response.Lcoe;

            // Loan and Mortgage will set MonthlyPayment as SolarPayment.
            SolarPayment = plan.Type == FinancePlanType.Loan || plan.Type == FinancePlanType.Mortgage || plan.Type == FinancePlanType.Lease
                ? response.MonthlyPayment : 0;

            Incentives = response.TotalBenefitsAndIncentives;
            TotalSavings = response.TotalSavings;

            TakeHomeIncentives = response.TotalTakeHomeIncentives;

            Name = plan.Type == FinancePlanType.Mortgage ? $"Mortgage {Terms} / {InterestRate.ToString("n2")}%" : plan.Name;

            TotalInterestPayment = response.TotalInterestPayment;
            LenderFee = plan.LenderFee != null ? plan.LenderFee.Value : 0;
            PPW = plan.PPW != null ? plan.PPW.Value : 0;
        }

        public Guid FinancePlanDefinitionId { get; set; }

        public string Name { get; set; }

        public FinancePlanType FinanceType { get; set; }

        public decimal FirstYearMonthlyElectricityCosts { get; set; }

        public decimal AverageMonthlyElectricityCosts { get; set; }

        public decimal MortgagePayment { get; set; }

        public decimal LoanPayment { get; set; }

        public decimal CombinedPayment => MortgagePayment + LoanPayment + FirstYearMonthlyElectricityCosts;

        public decimal SolarLoanPayment => LoanPayment;

        public decimal Incentives { get; set; }

        public decimal InterestRate { get; set; }

        public decimal TotalInterestPayment { get; set; }

        public decimal TakeHomeIncentives { get; set; }

        /// <summary>
        /// In Years
        /// </summary>
        public int Terms { get; set; }

        public int BreakEvenMonths { get; set; }

        public decimal Lcoe { get; set; }

        /// <summary>
        /// savings (or cost) for mortgage refi.  The difference between remaining interest that would be paid and new mortgage interest to be paid
        /// </summary>
        public decimal MortgageInterestSavings { get; set; }

        public decimal TotalSavings { get; set; }

        /// <summary>
        /// Amount that goes into solar out of loan or mortgage.
        /// </summary>
        public decimal SolarPayment { get; set; }

        /// <summary>
        /// Tells the client to show/hide the Build button
        /// </summary>
        public bool CanBuild { get; set; }
        public double LenderFee { get; set; } 
        public double PPW { get; set; }
        public decimal LenderFeesInAmount
        {
            get
            {
                if (LoanPayment != 0)
                { 
                    return Math.Round(LoanPayment * ((LoanPayment / (LoanPayment * (1 - (Convert.ToDecimal(LenderFee) / 100)))) - 1), 2); ;
                }
                else
                {
                    return 0;  
                } 
            }
        }
    }
}
