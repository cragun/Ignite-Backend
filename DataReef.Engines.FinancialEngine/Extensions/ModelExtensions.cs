using DataReef.TM.Models.DTOs.Solar;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataReef.Core.Extensions;
using DataReef.Engines.FinancialEngine.Models;
using DataReef.Integrations.LoanPal.Models.LoanPal;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;
using DataReef.Core;

public static class ModelExtensions
{
    public static decimal GetPricePerKWH(this EnergyMonthDetails month, int year, double inflationRate)
    {
        var pricePerKWH = month.Consumption != 0 ? month.PreSolarCost / month.Consumption : Math.Abs((month.PostSolarCost - month.PreSolarCost) / (month.Consumption - month.Production));
        return pricePerKWH.ApplyVariation(year, (decimal)inflationRate, true, 0);
    }

    public static decimal GetProduction(this EnergyMonthDetails month, int year, double derate, decimal productionIncrease)
    {
        return month.Production.ApplyVariation(year, (decimal)derate, false, productionIncrease);
    }

    public static decimal GetPreSolarCost(this EnergyMonthDetails month, int year, double inflationRate)
    {
        return month.PreSolarCost.ApplyVariation(year, (decimal)inflationRate, true, 0);
    }

    /// <summary>
    /// Get forecast Post Solar Cost
    /// </summary>
    /// <param name="month">Given Month</param>
    /// <param name="year">Forecast year</param>
    /// <param name="inflationRate">Utility Inflation Rate</param>
    /// <param name="derate">Annual System Degredation Rate</param>
    /// <returns></returns>
    public static decimal GetPostSolarCost(this EnergyMonthDetails month, int year, double inflationRate, double derate, decimal productionIncrease)
    {
        if (year == 1)
        {
            return month.PostSolarCost;
        }

        var pricePerKWH = month.GetPricePerKWH(year, inflationRate);
        var newProduction = month.GetProduction(year, derate, productionIncrease);
        var energyToBuy = month.PostAddersConsumptionOrConsumption - newProduction;

        return energyToBuy * pricePerKWH;
    }

    public static decimal GetPostSolarCostWithOrgOverride(this EnergyMonthDetails month, int year, double inflationRate, double derate, decimal productionIncrease, decimal? overridenUtilityRate, decimal? overridenUtilityBase)
    {
        if(overridenUtilityRate.HasValue && overridenUtilityBase.HasValue)
        {
            var newProduction = month.GetProduction(year, derate, productionIncrease);
            var energyToBuy = month.PostAddersConsumptionOrConsumption - newProduction;

            return energyToBuy * overridenUtilityRate.Value + overridenUtilityBase.Value;
        }
        return month.GetPostSolarCost(year, inflationRate, derate, productionIncrease);
    }

    public static bool CanBuildPlan(this FinancePlanType planType)
    {
        return planType == FinancePlanType.Cash
            || planType == FinancePlanType.Loan
            || planType == FinancePlanType.Mortgage;
    }

    public static PaymentMonth ToPaymentMonth(this AmortizationTableItem tableItem, LoanRequest request)
    {
        return new PaymentMonth
        {
            Month = tableItem.Period,
            Year = tableItem.Period.GetYear(),
            Principal = tableItem.Principal,
            PaymentAmount = tableItem.MonthlyPayment,
            PaymentAmountNoITC = tableItem.MonthlyPayment,
            PaymentBalance = tableItem.BeginningBalance,
            StartBalance = tableItem.BeginningBalance,
            EndBalance = tableItem.EndBalance,
            InterestCharge = tableItem.Interest,
            FederalTaxIncentive = tableItem.Period == FinancialEngineSettings.FederalTaxIncentiveMonth ? request.FederalTaxIncentive : 0,
            DealerIncentives = request.GetDealerIncentivesForMonth(tableItem.Period),
        };
    }

    public static LoanAmortizationData ToDRAmortization(this LoanCalculatorResponse model, LoanRequest request, FinancePlanDefinition planDefinition)
    {
        var response = new LoanAmortizationData
        {
            IntroMonthlyPayment = model.InitialMonthlyPaymentValue,
            MainLoanMonthlyPayment = model.AdjustedMonthlyPaymentValue,
            MainLoanMonthlyPaymentNoITC = model.AdjustedMonthlyPaymentValue,
            MainLoanApr = planDefinition.Apr,
            // We need to include 2'nd item in the Amortization table, even if it has the Monthly Payment = 0
            IntroPeriodInMonths = model?
                                    .AmortizationTable?
                                    .Where(at => at.MonthlyPayment == model.InitialMonthlyPaymentValue || (at.Period > 0 && at.MonthlyPayment == 0))?
                                    .Count() ?? 0,
            // We need to include last month, even if the Monthly Payment is less then AdjustedMonthlyPaymentValue 
            MainLoanPeriodInMonths = model?
                                    .AmortizationTable?
                                    .Where((at, idx) => at.MonthlyPayment == model.AdjustedMonthlyPaymentValue || idx == model?.AmortizationTable?.Count - 1)?
                                    .Count() ?? 0,
            Months = model?
                        .AmortizationTable
                        .Select(at => at.ToPaymentMonth(request))
                        .ToList(),
        };

        if (response.Months != null)
        {
            int scenarioTermInMonths = request.ScenarioTermInYears * 12;

            for (int monthIndex = response.Months.Count + 1; monthIndex <= scenarioTermInMonths; monthIndex++)
            {
                var year = monthIndex.GetYear();

                var paymentMonth = new PaymentMonth
                {
                    Year = year,
                    Month = monthIndex,
                    DealerIncentives = request.GetDealerIncentivesForMonth(monthIndex)
                };
                response.Months.Add(paymentMonth);
            }
        }
        return response;
    }
}

