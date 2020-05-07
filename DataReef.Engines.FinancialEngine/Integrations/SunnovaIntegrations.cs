using DataReef.Core;
using DataReef.Core.Extensions;
using DataReef.Engines.FinancialEngine.Models;
using DataReef.TM.Models.DataViews.Financing;
using DataReef.TM.Models.DataViews.Financing.Sunnova;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;
using System;
using System.Collections.Generic;

namespace DataReef.Engines.FinancialEngine.Integrations
{
    public class SunnovaIntegrations
    {

        public LoanAmortizationData BuildLoanAmortizationData(LoanRequest request, FinancePlanDefinition planDefinition)
        {
            var data = request
                            .GetPlanData<FinancePlanDataModel>()?
                            .Integrations?
                            .Sunnova;

            data = data ?? planDefinition.GetMetaData<FinancePlanDataModel>()?.Integrations?.Sunnova;

            if (data == null)
            {
                throw new ApplicationException($"No information was stored for the {planDefinition.Name} loan!");
            }

            int termInMonths = planDefinition.TermInMonths != 0 ? planDefinition.TermInMonths : planDefinition.TermInYears * 12;
            int scenarioTermInMonths = request.ScenarioTermInYears * 12;
            var amountToFinance = request.AmountToFinance;
            
            var months = new List<PaymentMonth>();
            decimal federalTaxIncentive = request.ApplyITCToLoan ? request.FederalTaxIncentive : 0;

            for (int monthIndex = 1; monthIndex <= termInMonths; monthIndex++)
            {
                decimal monthlyFederalTaxIncentive = monthIndex == FinancialEngineSettings.FederalTaxIncentiveMonth ? federalTaxIncentive : 0;

                var payingMonth = new PaymentMonth
                {
                    Year = monthIndex.GetYear(),
                    Month = monthIndex,
                    // finance details
                    PaymentAmount = data.GetMonthlyPayment(monthIndex, amountToFinance),
                    PaymentAmountNoITC = data.GetMonthlyPayment(monthIndex, amountToFinance),
                    // Incentives
                    FederalTaxIncentive = monthlyFederalTaxIncentive,
                    DealerIncentives = request.GetDealerIncentivesForMonth(monthIndex),
                };

                months.Add(payingMonth);
                //amountToFinance -= monthlyFederalTaxIncentive;
            }

            for (int monthIndex = termInMonths + 1; monthIndex <= scenarioTermInMonths; monthIndex++)
            {
                var payingMonth = new PaymentMonth
                {
                    Year = monthIndex.GetYear(),
                    Month = monthIndex,
                    DealerIncentives = request.GetDealerIncentivesForMonth(monthIndex),
                };
                months.Add(payingMonth);
            }

            return new LoanAmortizationData
            {
                IntroMonthlyPayment = data.GetMonthlyPayment(1, amountToFinance),
                MainLoanMonthlyPayment = data.GetMonthlyPayment(20, amountToFinance),
                MainLoanMonthlyPaymentNoITC = data.GetMonthlyPayment(20, amountToFinance),
                IntroPeriodInMonths = SunnovaFinancingPlanData.FirstPeriodNumberOfMonths,
                MainLoanPeriodInMonths = termInMonths - SunnovaFinancingPlanData.FirstPeriodNumberOfMonths,
                MainLoanApr = planDefinition.Apr,
                PaymentFactorsFirstPeriod = SunnovaFinancingPlanData.FirstPeriodNumberOfMonths,
                DeferredPeriodInMonths = 0,
                Months = months,          
            };
        }
    }
}
