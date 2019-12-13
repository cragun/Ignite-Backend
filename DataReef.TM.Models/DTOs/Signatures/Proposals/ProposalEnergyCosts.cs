using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    public class ProposalEnergyCosts
    {
        public ProposalEnergyCostItem WithoutSolar { get; set; }
        public ProposalEnergyCostItem WithSolar { get; set; }

        public List<ProposalEnergyCostItem> DynamicItems { get; set; }

        public ProposalEnergyCostItem Savings { get; set; }


        public List<ProposalEnergyCostYearData> AnnualData { get; set; }

        public int ScenarioTermInYears { get; set; }
        public double AnnualUtilityInflationRate { get; set; }

        public ProposalEnergyCosts() { }

        public ProposalEnergyCosts(FinancePlan financePlan, double? annualUtilityInflationRate, LoanRequest request, LoanResponse response, bool roundAmounts = false)
        {
            if (response == null || request == null)
            {
                return;
            }

            var lcoeNoSolar = response.TotalElectricityBillWithoutSolar / (decimal)response.TotalConsumption;

            var annualAverageWithoutSolar = response.TotalElectricityBillWithoutSolar / request.ScenarioTermInYears;
            var annualAverageWithSolar = response.TotalElectricityBillWithSolar / request.ScenarioTermInYears;

            WithoutSolar = new ProposalEnergyCostItem(response.FirstYearMonthlyElectricityBillWithoutSolar, annualAverageWithoutSolar, lcoeNoSolar, "Without Solar", roundAmounts: roundAmounts);
            WithSolar = new ProposalEnergyCostItem(response.FirstYearMonthlyElectricityBillWithSolar, annualAverageWithSolar, response.Lcoe, "With Solar", roundAmounts: roundAmounts);

            Savings = new ProposalEnergyCostItem
            {
                Label = "Savings",
                MonthlyAverage = WithoutSolar.MonthlyAverage - WithSolar.MonthlyAverage,
                AnnualAverage = WithoutSolar.AnnualAverage - WithSolar.AnnualAverage,
                CostOfEnergy = WithoutSolar.CostOfEnergy - WithSolar.CostOfEnergy,
            };

            ScenarioTermInYears = request.ScenarioTermInYears;
            AnnualUtilityInflationRate = annualUtilityInflationRate ?? (request.UtilityInflationRate * 100);

            AnnualData = response?
                            .Years?
                            .Select(y => new ProposalEnergyCostYearData(y, roundAmounts))?
                            .ToList();
            double sum = 0;
            AnnualData?.ForEach(ad =>
            {
                sum += ad.AnnualSavings;
                ad.CumulativeSavings = sum;
            });
        }
    }

    public class ProposalEnergyCostItem
    {
        public string Label { get; set; }
        public string CSSClass { get; set; }
        public double MonthlyAverage { get; set; }
        public double AnnualAverage { get; set; }
        public double CostOfEnergy { get; set; }        

        public ProposalEnergyCostItem() { }

        public ProposalEnergyCostItem(decimal monthlyBill, decimal annualAverage, decimal lcoe, string label, string cssClass = null, bool roundAmounts = false)
        {
            Label = label;
            CSSClass = cssClass;
            MonthlyAverage = roundAmounts ? Math.Round((double)monthlyBill) : (double)monthlyBill;
            AnnualAverage = roundAmounts ? Math.Round((double)annualAverage) : (double)annualAverage;
            CostOfEnergy = (double)lcoe;
        }
    }

    public class ProposalEnergyCostYearData
    {
        public int Year { get; set; }
        public double UtilityBillWithoutSolar { get; set; }
        public double UtilityBillWithSolar { get; set; }
        public double AnnualLoanPayment { get; set; }
        public double StartingLoanBalance { get; set; }
        public double Incentives { get; set; }
        public double AnnualSavings { get; set; }
        public double CumulativeSavings { get; set; }

        public ProposalEnergyCostYearData() { }

        public ProposalEnergyCostYearData(PaymentYear year, bool roundAmounts = false)
        {
            Year = year.Year;
            UtilityBillWithoutSolar = roundAmounts ? Math.Round((double)year.ElectricityBillWithoutSolar) : (double)year.ElectricityBillWithoutSolar;
            UtilityBillWithSolar = roundAmounts ? Math.Round((double)year.ElectricityBillWithSolar) : (double)year.ElectricityBillWithSolar;
            AnnualLoanPayment = roundAmounts ? Math.Round((double)year.PaymentAmount) : (double)year.PaymentAmount;
            StartingLoanBalance = roundAmounts ? Math.Round((double)year.StartBalance) : (double)year.StartBalance;
            Incentives = roundAmounts ? Math.Round((double)year.TotalBenefitsAndIncentives) : (double)year.TotalBenefitsAndIncentives;
            AnnualSavings = roundAmounts ? Math.Round((double)year.Savings) : (double)year.Savings;
        }
    }
}
