using DataReef.Core.Extensions;
using DataReef.Engines.FinancialEngine.Models;
using DataReef.TM.Models.DataViews.Financing;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;
using System;
using System.Collections.Generic;

namespace DataReef.Engines.FinancialEngine.Integrations
{
    public class PaymentFactorsIntegrations
    {
        public LoanAmortizationData BuildLoanAmortizationData(LoanRequest request, FinancePlanDefinition planDefinition)
        {
            var data = request
                            .GetPlanData<FinancePlanDataModel>()?
                            .Integrations?
                            .PaymentFactors;

            if (data == null && data.IsValid())
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
                decimal monthlyFederalTaxIncentive = monthIndex == data.ITCMonth ? federalTaxIncentive : 0;

                var payingMonth = new PaymentMonth
                {
                    Year = monthIndex.GetYear(),
                    Month = monthIndex,
                    // finance details
                    PaymentAmount = data.GetMonthlyPayment(monthIndex, amountToFinance),
                    PaymentAmountNoITC = data.GetMonthlyPayment(monthIndex, amountToFinance, false),
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
                IntroMonthlyPayment = data.GetMonthlyPayment(data.FirstPeriodMonths - 1, amountToFinance),
                MainLoanMonthlyPayment = data.GetMonthlyPayment(data.FirstPeriodMonths + 1, amountToFinance),
                MainLoanMonthlyPaymentNoITC = data.GetMonthlyPayment(data.FirstPeriodMonths + 1, amountToFinance, false),
                IntroPeriodInMonths = data.FirstPeriodMonths,
                PaymentFactorsFirstPeriod = data.FirstPeriodMonths,
                MainLoanPeriodInMonths = termInMonths - data.FirstPeriodMonths,
                MainLoanApr = planDefinition.Apr,
                DeferredPeriodInMonths = 0,
                Months = months,
            };
        }
    }
}
