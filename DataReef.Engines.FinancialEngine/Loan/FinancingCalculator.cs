using DataReef.Core.Attributes;
using DataReef.Core.Extensions;
using DataReef.Core.Logging;
using DataReef.Engines.FinancialEngine.Integrations;
using DataReef.Engines.FinancialEngine.Models;
using DataReef.Integrations.LoanPal;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.Engines.FinancialEngine.Loan
{
    public interface IFinancingCalculator
    {
        LoanResponse CalculateLoan(LoanRequest request, FinancePlanDefinition planDefinition);

        LoanResponse CalculateLease(LoanRequest request);

        List<FinanceEstimate> CompareFinancePlans(List<FinancePlanDefinition> plans, FinanceEstimateComparisonRequest request, Func<LoanRequest, FinancePlanDefinition, LoanResponse> enhancement = null);

        MortgageCalculationResponse CalculateMortgage(MortgageDetails request);
    }

    [Service(typeof(IFinancingCalculator))]
    public class FinancingCalculator : IFinancingCalculator
    {
        private readonly Lazy<ILoanPalBridge> _loanPalBridge;
        private readonly Lazy<ILogger> _logger;

        public FinancingCalculator(Lazy<ILoanPalBridge> loanPalBridge, Lazy<ILogger> logger)
        {
            _loanPalBridge = loanPalBridge;
            _logger = logger;
        }

        public MortgageCalculationResponse CalculateMortgage(MortgageDetails request)
        {
            //todo: calculate mortgage insurance
            double apr = request.Apr / 100;

            MortgageCalculationResponse response = new MortgageCalculationResponse();

            double pmt = request.Term == 0 ? 0 : Math.Abs(Financial.Pmt(apr / 12, request.Term, (double)request.OriginalBalance));
            double interest = 0;

            request.Date = request.Date == default(DateTime) ? DateTime.Now : request.Date;

            var startMonth = Math.Max(1, DateTime.Now.MonthsBetween(request.Date));

            for (int month = startMonth; month <= request.Term; month++)
            {
                interest += Financial.IPmt(apr / 12, month, request.Term, (double)request.OriginalBalance);
            }

            response.MonthlyPayment = Convert.ToDecimal(pmt);
            response.TotalInterest = Convert.ToDecimal(Math.Abs(interest));

            return response;
        }

        public LoanResponse CalculateLoan(LoanRequest request, FinancePlanDefinition planDefinition)
        {
            // Request validations
            if (request == null)
                throw new Exception("Loan Request Cannot Be Null");

            if (request.UpfrontRebate > 0 && request.SREC > 0)
                throw new Exception("Both: Upfront Rebate and SREC cannot be greater than 0 simultaneously!");

            if (request.UpfrontRebateReducedFromITC > request.UpfrontRebate)
                throw new Exception("Upfront Rebate Reduced from ITC should not be greater then Upfront Rebate!");

            decimal amountToFinance = request.AmountToFinance;

            // dirty, temporary hack to make the live app work w/ "Like Cash" plans, until it gets fixed on the iPad
            if (planDefinition.Type == FinancePlanType.Cash)
            {
                if (amountToFinance > 0)
                {
                    // The iPad is not adding the TotalAddersCosts to the DownPayment.
                    // in this case, we'll add it.
                    if (amountToFinance == request.TotalAddersCostsWithFinancingFee + request.TotalIncentives)
                    {
                        request.DownPayment += request.TotalAddersCostsWithFinancingFee + request.TotalIncentives;
                        amountToFinance = request.AmountToFinance;
                    }
                    // There are scenarios when both adders and incentives are present, and the iPad doesn't calclate it right.
                    else if (amountToFinance == request.TotalIncentives)
                    {
                        request.DownPayment += request.TotalIncentives;
                        amountToFinance = request.AmountToFinance;
                    }
                    else if (amountToFinance == (request.UpfrontRebate - request.UpfrontRebateReducedFromITC) + request.TotalIncentives)
                    {
                        amountToFinance = 0;
                    }
                    else
                    {
                        var json = JsonConvert.SerializeObject(request);
                        _logger.Value.Error("CASH request invalid: '{0}'", json);
                    }
                }
            }

            //if (planDefinition.Type == FinancePlanType.Cash && amountToFinance > 0)
            //{
            //    throw new ArgumentException("For cash plan, calculated amount to finance should be 0.");
            //}

            LoanAmortizationData amortizationData = null;

            switch (planDefinition.IntegrationProvider)
            {
                default:
                case FinancePlanIntegrationProvider.None:
                    amortizationData = BuildAmortizationData(request, planDefinition);
                    break;
                case FinancePlanIntegrationProvider.LoanPal:
                    //request.ApplyITCToLoan = false;
                    var loanPalIntegration = new LoanPalIntegrations(_loanPalBridge.Value);
                    amortizationData = loanPalIntegration.BuildLoanAmortizationData(request, planDefinition);
                    break;
                case FinancePlanIntegrationProvider.Sunnova:
                    var sunnovaIntegration = new SunnovaIntegrations();
                    amortizationData = sunnovaIntegration.BuildLoanAmortizationData(request, planDefinition);
                    break;
                case FinancePlanIntegrationProvider.SalalCreditUnion:
                    var salalCUIntegration = new SalalCreditUnionIntegrations();
                    amortizationData = salalCUIntegration.BuildLoanAmortizationData(request, planDefinition);
                    break;
                case FinancePlanIntegrationProvider.PaymentFactors:
                    var factorsIntegration = new PaymentFactorsIntegrations();
                    amortizationData = factorsIntegration.BuildLoanAmortizationData(request, planDefinition);
                    break;
            }

            var months = amortizationData.Months;
            var mainLoanApr = amortizationData.MainLoanApr;
            var deferredPeriodInMonths = amortizationData.DeferredPeriodInMonths;
            var introPeriodInMonths = amortizationData.IntroPeriodInMonths;
            var mainLoanPeriodInMonths = amortizationData.MainLoanPeriodInMonths;
            var introMonthlyPayment = amortizationData.IntroMonthlyPayment;
            var mainLoanMonthlyPayment = amortizationData.MainLoanMonthlyPayment;
            var mainLoanMonthlyPaymentNoITC = amortizationData.MainLoanMonthlyPaymentNoITC;

            #region Final adjustments for all months

            AdjustMonths(months, request);

            #endregion

            var years = CreateYears(months);

            var firstYear = years.First();

            var unappliedITC = request.ApplyITCToLoan ? 0 : request.FederalTaxIncentive;

            var response = new LoanResponse
            {
                Months = request.IncludeMonthsInResponse ? months : null,
                Years = years,

                AmountFinanced = amountToFinance.RoundValue(),
                SolarSystemCost = request.GrossSystemCostWithTaxAndDealerFee,
                TotalAddersCosts = request.TotalAddersCostsWithFinancingFee,
                AddersPaidByRep = request.AddersPaidByRep,
                TotalIncentives = request.TotalIncentives,

                StatedApr = (decimal)(mainLoanApr).RoundValue(),

                DeferredPeriodInMonths = deferredPeriodInMonths,
                IntroPeriodInMonths = introPeriodInMonths,
                PaymentFactorsFirstPeriod = amortizationData.PaymentFactorsFirstPeriod,
                MainLoanPeriodInMonths = mainLoanPeriodInMonths,

                IntroMonthlyPayment = introMonthlyPayment.RoundToUpparValue(),
                MonthlyPayment = mainLoanMonthlyPayment.RoundToUpparValue(),
                MonthlyPaymentNoITC = mainLoanMonthlyPaymentNoITC.RoundToUpparValue(),

                TotalFederalTaxIncentive = (years.Sum(y => y.FederalTaxIncentive) + unappliedITC).RoundValue(),
                TotalStateTaxIncentive = years.Sum(y => y.StateTaxIncentive).RoundValue(),
                TotalPBI = years.Sum(y => y.PBI).RoundValue(),
                TotalSREC = years.Sum(y => y.SREC).RoundValue(),
                TotalUpfrontRebate = request.UpfrontRebate,
                TotalUpfrontRebateReducedFromITC = request.UpfrontRebateReducedFromITC,
                TotalTakeHomeIncentives = ((years.Sum(y => y.TakeHomeIncentives) +
                                            (planDefinition.Type == FinancePlanType.Mortgage ? mainLoanMonthlyPayment : 0)).RoundValue() +
                                            years.Sum(y => y.DealerIncentives) +
                                            unappliedITC).RoundValue(),

                TotalBenefitsAndIncentives = (years.Sum(y => y.TotalBenefitsAndIncentives) + request.UpfrontRebate + unappliedITC).RoundValue(),

                TotalSystemProduction = (long)years.Sum(y => y.SystemProduction),
                TotalConsumption = years.Sum(y => y.Consumption),
                PostAddersConsumption = years.Sum(y => y.PostAddersConsumption),
                TotalElectricityBillWithoutSolar = years.Sum(y => y.ElectricityBillWithoutSolar).RoundValue(),
                TotalElectricityBillWithSolar = years.Sum(y => y.ElectricityBillWithSolar).RoundValue(),

                FirstYearMonthlyElectricityBillWithoutSolar = (firstYear.ElectricityBillWithoutSolar / 12).RoundValue(),
                FirstYearMonthlyElectricityBillWithSolar = (firstYear.ElectricityBillWithSolar / 12).RoundValue(),
                AvgMonthlyElectrityBillWithSolar = (years.Average(y => y.ElectricityBillWithSolar) / 12).RoundValue(),
                AvgMonthlyElectrityBillWithoutSolar = (years.Average(y => y.ElectricityBillWithoutSolar) / 12).RoundValue(),

                TotalInterestPayment = years.Sum(y => y.InterestCharge).RoundValue(),
                TotalSolarPaymentsCost = (years.Sum(y => y.PaymentAmount) + request.DownPayment + request.UpfrontRebate).RoundValue(),
                TotalSavings = (years.Sum(y => y.Savings) - request.DownPayment + unappliedITC).RoundValue(),
            };

            //TODO: Check w/ Jason if we need to subtract incentives from total principal
            var totalPrincipal = (years.Sum(y => y.Principal) + response.Years.Last().UnpaidInternalBalance);
            if (totalPrincipal != 0)
            {
                var totalInterest = years.Sum(y => y.InterestCharge);
                response.ActualApr = ((totalInterest / totalPrincipal) * 100).RoundValue();
            }
            return response;
        }

        public LoanResponse CalculateLease(LoanRequest request)
        {
            var months = new List<PaymentMonth>();

            var escalator = request.LeaseEscalator;
            var monthlyTax = request.LeaseParams?.MonthlyLeaseTax ?? request.MonthlyLeaseTax;

            //decimal firstYearAnnualProduction = request.MonthlyPower.Sum(mp => mp.Production);
            //decimal firstYearAnnualConsumption = request.MonthlyPower.Sum(mp => mp.Consumption);

            decimal grossSystemCostWithTax = request.LeaseSystemCostWithExtrasAndTax;
            decimal yearlyCost = grossSystemCostWithTax;
            decimal year1MonthlyCost = 0;
            decimal monthlyCost = 0;
            var orderedMonthPower = request
                                        .MonthlyPower
                                        .OrderBy(mp => mp.Month)
                                        .ToList();

            for (int yearIndex = 0; yearIndex < request.ScenarioTermInYears; yearIndex++)
            {
                // starting year 1, will add Escalator
                if (yearIndex > 0)
                {
                    yearlyCost = yearlyCost * (1 + (request.LeaseEscalator / 100));
                }

                // calculate the monthly cost and add Monthly lease tax to it.
                monthlyCost = (yearlyCost / 12) * (1 + (request.MonthlyLeaseTax / 100));

                for (int monthIndex = 1; monthIndex <= 12; monthIndex++)
                {
                    var paymentMonth = new PaymentMonth
                    {
                        Year = yearIndex,
                        Month = (yearIndex * 12) + monthIndex,
                        PaymentAmount = monthlyCost,
                    };
                    months.Add(paymentMonth);
                }

                if (year1MonthlyCost == 0)
                {
                    year1MonthlyCost = monthlyCost;
                }
            }

            AdjustMonths(months, request);

            var years = CreateYears(months);

            var firstYear = years.First();

            var response = new LoanResponse
            {
                Months = request.IncludeMonthsInResponse ? months : null,
                Years = years,
                MonthlyPayment = year1MonthlyCost,

                SolarSystemCost = grossSystemCostWithTax,
                TotalAddersCosts = request.TotalAddersCostsWithFinancingFee,
                AddersPaidByRep = request.AddersPaidByRep,
                TotalIncentives = request.TotalIncentives,

                TotalPBI = years.Sum(y => y.PBI).RoundValue(),
                TotalSREC = years.Sum(y => y.SREC).RoundValue(),
                TotalUpfrontRebate = request.UpfrontRebate,
                TotalUpfrontRebateReducedFromITC = request.UpfrontRebateReducedFromITC,

                TotalSystemProduction = (long)years.Sum(y => y.SystemProduction),
                TotalConsumption = years.Sum(y => y.Consumption),
                PostAddersConsumption = years.Sum(y => y.PostAddersConsumption),
                TotalElectricityBillWithoutSolar = years.Sum(y => y.ElectricityBillWithoutSolar).RoundValue(),
                TotalElectricityBillWithSolar = years.Sum(y => y.ElectricityBillWithSolar).RoundValue(),

                FirstYearMonthlyElectricityBillWithoutSolar = (firstYear.ElectricityBillWithoutSolar / 12).RoundValue(),
                FirstYearMonthlyElectricityBillWithSolar = (firstYear.ElectricityBillWithSolar / 12).RoundValue(),
                AvgMonthlyElectrityBillWithSolar = (years.Average(y => y.ElectricityBillWithSolar) / 12).RoundValue(),
                AvgMonthlyElectrityBillWithoutSolar = (years.Average(y => y.ElectricityBillWithoutSolar) / 12).RoundValue(),

                TotalSolarPaymentsCost = (years.Sum(y => y.PaymentAmount) + request.DownPayment + request.UpfrontRebate).RoundValue(),
                TotalSavings = (years.Sum(y => y.Savings) - request.DownPayment).RoundValue(),
            };

            return response;
        }

        private Tuple<int, double> GetPartAndDiff(List<double> items, double value)
        {
            double sum = 0;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var newSum = sum + item;
                if (newSum > value)
                {
                    return new Tuple<int, double>(i, value - sum);
                }

                sum = newSum;
            }
            return new Tuple<int, double>(items.Count, 0);
        }

        private void AdjustMonths(List<PaymentMonth> months, LoanRequest request)
        {
            var orderedMonthPower = request.OrderedMonthPower;
            foreach (var paymentMonth in months)
            {
                // Calculate PBI & SREC and add them to the month benefits (TakeHome & TotalBenefits)
                var monthIndex = paymentMonth.Month;
                var year = paymentMonth.Year;

                int calendarMonth = monthIndex.GetCalendarMonth();
                var monthPower = orderedMonthPower[calendarMonth];

                // energy details
                paymentMonth.SystemProduction = (double)monthPower.GetProduction(year, request.Derate, (request.LeaseParams?.ProductionIncrease ?? 0));
                paymentMonth.Consumption = (double)monthPower.Consumption;
                paymentMonth.PostAddersConsumption = (double)monthPower.PostAddersConsumption;
                paymentMonth.ElectricityBillWithoutSolar = monthPower.GetPreSolarCost(year, request.UtilityInflationRate);
                paymentMonth.ElectricityBillWithSolar = monthPower.GetPostSolarCostWithOrgOverride(year, request.UtilityInflationRate, request.Derate, request.LeaseParams?.ProductionIncrease ?? 0, request.OverridenUtilityRate, request.OverridenUtilityBaseFee);

                if (monthIndex >= 2 &&
                    monthIndex <= (1 + request.PBITermInYears * 12))
                {
                    paymentMonth.PBI = (decimal)paymentMonth.SystemProduction * request.PBI;
                }
                if (monthIndex % 12 == 1 &&
                    paymentMonth.Year >= 2 &&
                    paymentMonth.Year <= (1 + request.SRECTermInYears))
                {
                    var lastYearProduction = months
                                                .Where(m => m.Year == (paymentMonth.Year - 1))
                                                .Sum(m => m.SystemProduction);
                    paymentMonth.SREC = (decimal)lastYearProduction * request.SREC;
                }

                paymentMonth.TakeHomeIncentives += paymentMonth.PBI + paymentMonth.SREC;
            }
        }

        private LoanAmortizationData BuildAmortizationData(LoanRequest request, FinancePlanDefinition planDefinition)
        {
            int termInMonths = planDefinition.TermInMonths == 0 ? planDefinition.TermInYears * 12 : planDefinition.TermInMonths;
            int scenarioTermInMonths = request.ScenarioTermInYears * 12;
            // number of months left to finance for plan details
            int monthsLeftToFinance = termInMonths;

            // incentives
            decimal federalTaxIncentive = request.ApplyITCToLoan ? request.FederalTaxIncentive : 0;
            decimal stateTaxIncentive = request.StateTaxIncentive;
            decimal takeHomeIncentives = 0;

            decimal amountToFinance = request.AmountToFinance;

            var orderedMonthPower = request.OrderedMonthPower;

            // amount payed when nor interest and principal are not paid
            decimal introMonthlyPayment = 0;
            decimal mainLoanMonthlyPayment = 0;

            var months = new List<PaymentMonth>();

            planDefinition.Details = planDefinition.Details ?? new List<FinanceDetail>();
            // Calculate & create payment months for each loan detail
            var orderedFinanceDetails = planDefinition.Details.OrderBy(fd => fd.Order).ToList();

            double mainLoanApr = 0;
            // number of months the client doesn't pay intrest nor principal
            int deferredPeriodInMonths = 0;
            int introPeriodInMonths = 0;
            int mainLoanPeriodInMonths = 0;

            var balance = amountToFinance;
            // Spruce states that it has a no interest / no principal period, but they hide interest for that period.
            double deferredInterest = 0;
            // Some financep roviders will subtract the ITC from amount to finance, but if the customer will cash in the ITC
            // they will charge the interest for the first 18 months for ITC
            double accumulatedITCInterest = 0;
            var detailBalance = balance;
            var scenarioMonth = 1;
            // This covers the scenario (Sunnova) in which the company calculates the payment for first 18 months for the whole amount - ITC
            // but if the user chooses to cash in the ITC, the finance company, adds back the ITC in the balance starting month 19th
            // and it also adds the interest for ITC for the first 18 months
            var needToAddTheITCToBalance = false;

            // iterate through plan details
            foreach (var detail in orderedFinanceDetails)
            {
                // Make sure we cover both cases, when apr is sent like: 3.99 and when it's sent like: 0.0399
                decimal apr = (decimal)(detail.Apr > 0.5 ? detail.Apr / 100 : detail.Apr);
                decimal monthlyApr = apr / 12;

                decimal federalTaxIncentiveApplied = 0;
                decimal stateTaxIncentiveApplied = 0;

                var isLastDetail = detail == orderedFinanceDetails.Last();

                if (needToAddTheITCToBalance)
                {
                    balance += federalTaxIncentive;
                    detailBalance += federalTaxIncentive;
                    needToAddTheITCToBalance = false;
                }

                switch (detail.ApplyReductionAfterPeriod)
                {
                    case ReductionType.ReduceInTheBeginning:
                        federalTaxIncentiveApplied = federalTaxIncentive;
                        stateTaxIncentiveApplied = stateTaxIncentive;

                        balance -= federalTaxIncentive;
                        detailBalance -= federalTaxIncentive;
                        amountToFinance -= federalTaxIncentive;
                        break;
                    case ReductionType.ReduceInTheBeginningButAccumulateInterest:
                        federalTaxIncentiveApplied = federalTaxIncentive;
                        stateTaxIncentiveApplied = stateTaxIncentive;

                        balance -= federalTaxIncentive;
                        detailBalance -= federalTaxIncentive;
                        needToAddTheITCToBalance = true;
                        break;
                    case ReductionType.ReduceInTheBeginningUsingRebatesIncentives:
                        amountToFinance -= request.RebateIncentives;
                        balance -= request.RebateIncentives;
                        detailBalance -= request.RebateIncentives;
                        break;
                    default:
                        break;
                }

                // go through every month in the plan details
                for (int monthIndex = 1; monthIndex <= detail.Months; monthIndex++)
                {
                    double interest = 0;
                    double principal = 0;
                    double deferredInterestPayment = 0;
                    var isFirstMonth = monthIndex == 1;
                    var isLastMonth = monthIndex == detail.Months;

                    decimal monthFederalTaxIncentive = 0;
                    decimal monthStateTaxIncentive = 0;

                    // for scenarios when we reduce the federal Tax Incentive before processing months,
                    // we add it to the first month
                    if (isFirstMonth)
                    {
                        if (federalTaxIncentiveApplied > 0)
                        {
                            monthFederalTaxIncentive = federalTaxIncentiveApplied;
                            federalTaxIncentiveApplied = 0;
                        }
                        if (stateTaxIncentiveApplied > 0)
                        {
                            monthStateTaxIncentive = stateTaxIncentiveApplied;
                            stateTaxIncentiveApplied = 0;
                        }
                    }

                    var year = scenarioMonth.GetYear();

                    // get the actual calendaristic month
                    int calendarMonth = scenarioMonth.GetCalendarMonth();
                    var monthPower = orderedMonthPower[calendarMonth];

                    switch (detail.InterestType)
                    {
                        case InterestType.OutOfLoan:
                        case InterestType.Regular:
                            {
                                // If will apply reduction at the end of the period, calculate interest 
                                // without considering scenarioMonth
                                if (detail.PrincipalType == PrincipalType.None)
                                {
                                    interest = (double)(balance * monthlyApr);
                                }
                                else
                                {
                                    interest = Math.Abs(Financial.IPmt((double)monthlyApr, monthIndex, monthsLeftToFinance, (double)detailBalance));
                                }
                                break;
                            }
                        case InterestType.Deferred:
                            {
                                deferredInterest += (double)(balance * monthlyApr);
                                break;
                            }
                    }

                    if (detail.PrincipalType == PrincipalType.Loan)
                    {
                        principal = Math.Abs(Financial.PPmt((double)monthlyApr, monthIndex, monthsLeftToFinance, (double)detailBalance));

                        if (deferredInterest > 0)
                        {
                            deferredInterestPayment = Math.Min(deferredInterest, principal);
                            principal = principal - deferredInterestPayment;
                            deferredInterest -= deferredInterestPayment;
                        }
                    }

                    // In this scenario, the finance company calculates the payment after it subtracts the ITC
                    // but it accumulates the interest for the ITC and it will add it to next period
                    if (detail.ApplyReductionAfterPeriod == ReductionType.ReduceInTheBeginningButAccumulateInterest)
                    {
                        accumulatedITCInterest += (double)(federalTaxIncentive * monthlyApr);
                    }

                    var monthlyPayment = (decimal)(interest + principal + deferredInterestPayment);

                    if (isLastMonth)
                    {
                        switch (detail.ApplyReductionAfterPeriod)
                        {
                            case ReductionType.PrincipalReduction:
                                {
                                    monthFederalTaxIncentive = federalTaxIncentive;
                                    monthStateTaxIncentive = stateTaxIncentive;

                                    var incentivesToApply = monthFederalTaxIncentive + monthStateTaxIncentive;

                                    // when Spruced:
                                    // - the interest will be subtracted from the incentives even if it's less than the incentives
                                    // - the monthly payment should not exceed the total Incentives
                                    // - what's left of the deferredInterest (not covered by the incentives), will be used in the next planDetail to calculate interest
                                    if (detail.IsSpruced)
                                    {
                                        if (incentivesToApply > 0 && incentivesToApply < monthlyPayment)
                                        {
                                            monthlyPayment = incentivesToApply;

                                            var data = GetPartAndDiff(new List<double> { interest, principal }, (double)incentivesToApply);
                                            switch (data.Item1)
                                            {
                                                case 0:
                                                    deferredInterest += (interest - data.Item2) + principal;
                                                    principal = 0;
                                                    break;
                                                case 1:
                                                    deferredInterest += (principal - data.Item2);
                                                    principal = data.Item2;
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            incentivesToApply -= (decimal)interest;
                                            principal = Math.Max((double)incentivesToApply - deferredInterest, 0);
                                            deferredInterest = Math.Max(deferredInterest - (double)incentivesToApply, 0);
                                        }
                                    }
                                    else
                                    {
                                        principal += (double)incentivesToApply;
                                    }

                                    monthlyPayment = (decimal)(interest + principal + deferredInterestPayment);
                                    break;
                                }
                            case ReductionType.TakeHome:
                                {
                                    monthFederalTaxIncentive = federalTaxIncentive;
                                    monthStateTaxIncentive = stateTaxIncentive;

                                    takeHomeIncentives += monthFederalTaxIncentive + monthStateTaxIncentive;
                                    break;
                                }
                        }
                    }

                    balance -= (decimal)principal;

                    // set the balance to 0 if it's smaller than 0.5 / 100 monetary units
                    balance = balance <= 0.005m ? 0 : balance;

                    if (detail.PrincipalType == PrincipalType.None &&
                        (detail.InterestType == InterestType.Deferred || detail.InterestType == InterestType.None))
                    {
                        deferredPeriodInMonths++;
                    }
                    else if (detail.PrincipalType == PrincipalType.None &&
                        detail.InterestType == InterestType.Regular)
                    {
                        introPeriodInMonths++;
                        if (introMonthlyPayment == 0)
                        {
                            introMonthlyPayment = monthlyPayment;
                        }
                    }
                    else if (detail.PrincipalType == PrincipalType.Loan &&
                       detail.InterestType == InterestType.Regular)
                    {
                        mainLoanPeriodInMonths++;
                        // only store the monthlyPayment on a non-introductory period
                        if (mainLoanMonthlyPayment == 0 && isLastDetail)
                        {
                            mainLoanMonthlyPayment = monthlyPayment;
                            mainLoanApr = detail.Apr;
                        }
                    }

                    var payingMonth = new PaymentMonth
                    {
                        Year = year,
                        Month = scenarioMonth,
                        // finance details
                        PaymentAmount = monthlyPayment,
                        PaymentAmountNoITC = monthlyPayment,
                        Principal = (decimal)principal,
                        InterestCharge = (decimal)interest,
                        PaymentBalance = balance,
                        UnpaidInternalBalance = (decimal)deferredInterest,
                        // Incentives
                        FederalTaxIncentive = monthFederalTaxIncentive,
                        StateTaxIncentive = monthStateTaxIncentive,
                        TakeHomeIncentives = takeHomeIncentives,
                        DealerIncentives = request.GetDealerIncentivesForMonth(scenarioMonth)
                    };

                    months.Add(payingMonth);
                    scenarioMonth += 1;
                }

                if (detail.InterestType == InterestType.Deferred ||
                    (detail.InterestType == InterestType.OutOfLoan && detail.PrincipalType == PrincipalType.None))
                {
                    // don't reduce the monthsLeftToFinance
                }
                else
                {
                    // subtract the number of months covered by this plan details
                    monthsLeftToFinance -= detail.Months;
                }

                //if (detail.InterestType != InterestType.Deferred ||
                //    // Don't subtract the number of months from monthsLeftToFinance if the Interest is OutOfLoan and no Principal is paied
                //    (detail.InterestType != InterestType.OutOfLoan && detail.PrincipalType == PrincipalType.None))
                //{
                //    // subtract the number of months covered by this plan details
                //    monthsLeftToFinance -= detail.Months;
                //}

                // at the end of a period, prepare the balance for next period
                detailBalance = balance + (decimal)deferredInterest + (decimal)accumulatedITCInterest;
                accumulatedITCInterest = 0;
            }

            // Add the rest of the months to reach the scenario term
            for (int monthIndex = scenarioMonth; monthIndex <= scenarioTermInMonths; monthIndex++)
            {
                var year = monthIndex.GetYear();

                var paymentMonth = new PaymentMonth
                {
                    Year = year,
                    Month = monthIndex,
                    DealerIncentives = request.GetDealerIncentivesForMonth(monthIndex)
                };
                months.Add(paymentMonth);
            }

            return new LoanAmortizationData
            {
                Months = months,
                MainLoanApr = mainLoanApr,
                DeferredPeriodInMonths = deferredPeriodInMonths,
                IntroMonthlyPayment = introMonthlyPayment,
                IntroPeriodInMonths = introPeriodInMonths,
                MainLoanMonthlyPayment = mainLoanMonthlyPayment,
                MainLoanMonthlyPaymentNoITC = mainLoanMonthlyPayment,
                MainLoanPeriodInMonths = mainLoanPeriodInMonths
            };
        }

        private List<PaymentYear> CreateYears(List<PaymentMonth> months)
        {
            var monthsGroupedByYear = months.GroupBy(m => m.Year).ToList();
            var years = monthsGroupedByYear
                            .Select(mg => new PaymentYear
                            {
                                Year = mg.Key,

                                Principal = mg.Sum(pm => pm.Principal),
                                PaymentAmount = mg.Sum(pm => pm.PaymentAmount),
                                PaymentAmountNoITC = mg.Sum(pm => pm.PaymentAmountNoITC),
                                PaymentBalance = mg.LastOrDefault().PaymentBalance,
                                StartBalance = mg.FirstOrDefault().StartBalance == 0 ? mg.FirstOrDefault().BalanceAndPrincipal : mg.FirstOrDefault().StartBalance,
                                EndBalance = mg.LastOrDefault().EndBalance == 0 ? mg.LastOrDefault().PaymentBalance : mg.LastOrDefault().EndBalance,
                                UnpaidInternalBalance = mg.LastOrDefault().UnpaidInternalBalance,
                                InterestCharge = mg.Sum(pm => pm.InterestCharge),

                                SREC = mg.Sum(pm => pm.SREC),
                                PBI = mg.Sum(pm => pm.PBI),
                                StateTaxIncentive = mg.Sum(pm => pm.StateTaxIncentive),
                                FederalTaxIncentive = mg.Sum(pm => pm.FederalTaxIncentive),
                                TakeHomeIncentives = mg.Sum(pm => pm.TakeHomeIncentives),
                                DealerIncentives = mg.Sum(pm => pm.DealerIncentives),

                                SystemProduction = mg.Sum(pm => pm.SystemProduction),
                                Consumption = mg.Sum(pm => pm.Consumption),
                                PostAddersConsumption = mg.Sum(pm => pm.PostAddersConsumption),
                                ElectricityBillWithoutSolar = mg.Sum(pm => pm.ElectricityBillWithoutSolar),
                                ElectricityBillWithSolar = Math.Max(mg.Sum(pm => pm.ElectricityBillWithSolar), 0), // don't add negative values
                            })
                            .ToList();
            return years;
        }
        public List<FinanceEstimate> CompareFinancePlans(List<FinancePlanDefinition> plans, FinanceEstimateComparisonRequest request, Func<LoanRequest, FinancePlanDefinition, LoanResponse> enhancement = null)
        {
            var result = new List<FinanceEstimate>();

            // initial calculations
            MortgageCalculationResponse currentMortgage = null;

            if (request?.MortgageData?.CurrentMortgage != null)
            {
                currentMortgage = CalculateMortgage(request.MortgageData.CurrentMortgage);
            }
            decimal currentMortgagePayment = (currentMortgage?.MonthlyPayment ?? 0).RoundValue();
            decimal interestRate = Convert.ToDecimal(request?.MortgageData?.CurrentMortgage?.Apr ?? 0);

            var firstYearAverageMonthlyElectricityCosts = request.MonthlyPower.Average(m => m.PreSolarCost).RoundValue();

            // Always add the "Do Nothing" estimate
            var noneEstimate = new FinanceEstimate()
            {
                FinanceType = FinancePlanType.None,
                Name = "Do Nothing",
                MortgagePayment = currentMortgagePayment,
                CanBuild = false,
                FirstYearMonthlyElectricityCosts = firstYearAverageMonthlyElectricityCosts,
                AverageMonthlyElectricityCosts = firstYearAverageMonthlyElectricityCosts,
                Lcoe = request.FirstYearElectricityCosts / request.FirstYearElectricityConsumption
            };
            result.Add(noneEstimate);

            foreach (var financePlanDefinition in plans)
            {
                // cloning the request object
                var clonedRequest = JsonConvert.DeserializeObject<FinanceEstimateComparisonRequest>(JsonConvert.SerializeObject(request));
                clonedRequest.DealerFee = request.GetDealerFee(financePlanDefinition.Guid);
                clonedRequest.FinancePlanData = request.GetPlanData(financePlanDefinition.Guid);

                switch (financePlanDefinition.Type)
                {
                    case FinancePlanType.Cash:
                        {
                            // AmountToFinance is calculated using DownPayment. We set it to 0 before getting the amountToFinance to make sure it doesn't interfere.
                            clonedRequest.DownPayment = 0;
                            clonedRequest.DownPayment = clonedRequest.AmountToFinance;

                            var cashResponse = CalculateLoan(clonedRequest, financePlanDefinition);
                            var cashEstimate = new FinanceEstimate(cashResponse, financePlanDefinition, currentMortgagePayment);

                            result.Add(cashEstimate);
                            break;
                        }
                    case FinancePlanType.Loan:
                        {
                            try
                            {
                                var loanResponse = CalculateLoan(clonedRequest, financePlanDefinition);

                                var enhancedResponse = enhancement?.Invoke(clonedRequest, financePlanDefinition);
                                if (enhancedResponse != null)
                                {
                                    loanResponse.MonthlyPayment = enhancedResponse.MonthlyPayment;
                                }

                                var loanEstimate = new FinanceEstimate(loanResponse, financePlanDefinition, currentMortgagePayment);

                                result.Add(loanEstimate);
                            }
                            catch (Exception ex)
                            {
                                _logger.Value.Error(ex.Message, ex);
                            }
                            break;
                        }
                    case FinancePlanType.Lease:
                        {
                            try
                            {
                                var leaseResponse = CalculateLease(clonedRequest);
                                var leaseEstimate = new FinanceEstimate(leaseResponse, financePlanDefinition, currentMortgagePayment);

                                result.Add(leaseEstimate);
                            }
                            catch (Exception ex)
                            {
                                _logger.Value.Error(ex.Message, ex);
                            }
                            break;
                        }
                    case FinancePlanType.Mortgage:
                        {
                            if (currentMortgage != null &&
                                clonedRequest.MortgageData.ProposedMortgages != null)
                            {
                                foreach (var proposedMortgage in clonedRequest.MortgageData.ProposedMortgages)
                                {
                                    // calculate the mortgage for the entire loan (solar + current mortgage)
                                    //var mortgageRequest = new LoanRequest2(request);
                                    financePlanDefinition.SetMortgageDetails(proposedMortgage);
                                    clonedRequest.IncludeAmountToRefinance = true; // = request.MortgageData.CurrentMortgage.CurrentBalance + request.MortgageData.ClosingCostRate;
                                    var entireMortgageResponse = CalculateLoan(clonedRequest, financePlanDefinition);

                                    // calculate solar portion of the mortgage. 
                                    clonedRequest.IncludeAmountToRefinance = false;

                                    var solarMortgageResponse = CalculateLoan(clonedRequest, financePlanDefinition);
                                    // TakeHomeIncentives should be taken from the Entire Mortgage calculation
                                    solarMortgageResponse.TotalTakeHomeIncentives = entireMortgageResponse.TotalTakeHomeIncentives;

                                    var mortgageEstimate = new FinanceEstimate(solarMortgageResponse, financePlanDefinition, entireMortgageResponse.MonthlyPayment);

                                    // Calculate the difference between current mortgage total Interest & proposed mortgage iterest 
                                    mortgageEstimate.MortgageInterestSavings = currentMortgage.TotalInterest - entireMortgageResponse.TotalInterestPayment;
                                    mortgageEstimate.TotalSavings += Math.Max(mortgageEstimate.MortgageInterestSavings, 0);

                                    result.Add(mortgageEstimate);
                                }
                            }
                            break;
                        }
                }
            }

            return result;
        }
    }
}
