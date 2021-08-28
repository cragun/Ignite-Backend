using DataReef.Core;
using DataReef.Core.Attributes;
using DataReef.Engines.FinancialEngine.Loan;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.DTOs.Signatures.Proposals;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using DataReef.TM.Models.Solar;
using DataReef.TM.Services.Services.ProposalAddons.TriSMART.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Services.Services.ProposalAddons.TriSMART
{
    [Service(typeof(ITrismartProposalEnhancement))]
    public class TrismartProposalEnhancement : ITrismartProposalEnhancement
    {

        public const string SubPlan_Standard = "Standard";
        public const string SubPlan_Smart = "Smart";
        public const string SubPlan_Smarter = "Smarter";

        private IFinancingCalculator _loanCalculator;
        private IOUSettingService _settingsService;
        public TrismartProposalEnhancement(IFinancingCalculator loanCalculator, IOUSettingService settingsService)
        {
            _loanCalculator = loanCalculator;
            _settingsService = settingsService;
        }

        public void EnhanceProposalData(Proposal2DataView proposal, TriSmartConstructor param, bool roundAmounts = false)
        {
            AddSummary(proposal, param);
            var planDefinition = param?.FinancePlan?.FinancePlanDefinition;

            //if (planDefinition?.IntegrationProvider == FinancePlanIntegrationProvider.LoanPal)
            //{
            //    AddLoanSummary(proposal, param);
            //}

            if (proposal.Financing.FinancingType == FinancePlanType.Lease)
            {
                AddLeaseFinanceOptions(proposal, param, roundAmounts);
            }
            else
            {
                AddLoanSummary(proposal, param);
            }
        }

        private void AddFinancePlanOptions(Proposal2DataView proposal, TriSmartConstructor param)
        {
            var planDefinition = param?.FinancePlan?.FinancePlanDefinition;

            var first = param
                        .FinancePlan
                        .FinancePlanDefinition
                        .Details
                        .ToList()
                        .FirstOrDefault(d => d.ApplyReductionAfterPeriod != ReductionType.None);
            if (first != null)
            {
                AddSpecialLoan(proposal, param, first);
            }

            if (planDefinition?.IntegrationProvider == FinancePlanIntegrationProvider.LoanPal)
            {
                AddLoanSummary(proposal, param);
            }

            if (proposal.Financing.FinancingType == FinancePlanType.Lease)
            {
                AddLeaseFinanceOptions(proposal, param);
            }
        }

        private void AddLeaseFinanceOptions(Proposal2DataView proposal, TriSmartConstructor param, bool roundAmounts = false)
        {
            var financePlanDefinition = param.FinancePlan.FinancePlanDefinition;
            var proposalData = param.Data;
            var financePlan = param.FinancePlan;

            var smarterRequest = financePlan.GetRequest(true);
            smarterRequest.UtilityInflationRate = param.UtilityInflationRate ?? smarterRequest.UtilityInflationRate;
            smarterRequest.ScenarioTermInYears = 30;
            smarterRequest.IncludeMonthsInResponse = true;

            var smarterResponse = _loanCalculator.CalculateLease(smarterRequest);
            var smarterPlan = smarterResponse.ToPlanOption("SMARTER", "Apply All Incentives and Savings", PlanOptionType.Smarter);
            smarterPlan.Balance = 0;

            proposal.FinancePlanOptions = new List<ProposalFinancePlanOption> { smarterPlan };
            proposal.Financing.MonthlyPayment = roundAmounts ? Math.Round((double)smarterResponse.MonthlyPayment) : (double)smarterResponse.MonthlyPayment;
        }

        private void AddLoanSummary(Proposal2DataView proposal, TriSmartConstructor param)
        {
            var scenarioTermInYears = 30;
            var proposalData = param.Data;
            var financePlan = param.FinancePlan;
            var planDefinition = financePlan.FinancePlanDefinition;
            decimal introMonthlyPayment = 0M;

            decimal? overridenUtilityRate = null, overridenUtilityBaseRate = null;
            if (param.Proposal.OUID.HasValue && param.Proposal != null && param.Proposal.Tariff != null)
            {
                var orgSettings = _settingsService.GetSettings(param.Proposal.OUID.Value, null);
                List<CustomUtilityRate> customUtilityRates = null;

                var sett = orgSettings.GetByKey(OUSetting.Utility_Rates);
                if (!string.IsNullOrEmpty(sett))
                {
                    customUtilityRates = JsonConvert.DeserializeObject<List<CustomUtilityRate>>(sett);
                }

                var customUtilityRate = customUtilityRates
                    ?.FirstOrDefault(x =>
                    x.UtilityProvider.Equals(param.Proposal.Tariff.TariffID, StringComparison.CurrentCultureIgnoreCase)
                    || x.UtilityProvider.Equals(param.Proposal.Tariff.UtilityName, StringComparison.CurrentCultureIgnoreCase));

                if (customUtilityRate != null)
                {
                    overridenUtilityRate = customUtilityRate.UtilityRate;
                    overridenUtilityBaseRate = customUtilityRate.UtilityBaseFee;
                }
            }

            #region Standard Plan

            var stdRequest = financePlan.GetRequest(true);
            stdRequest.UtilityInflationRate = param.UtilityInflationRate ?? stdRequest.UtilityInflationRate;
            stdRequest.ScenarioTermInYears = scenarioTermInYears;
            stdRequest.IncludeMonthsInResponse = true;

            // get the incentives for Standard plan.

            var stdIncentiveValues = GetIncentivesTotalForPlan(stdRequest, SubPlan_Standard);

            stdRequest.UpfrontRebate = stdIncentiveValues.Item1 + stdIncentiveValues.Item2;
            stdRequest.UpfrontRebateReducedFromITC = stdIncentiveValues.Item1;
            stdRequest.OverridenUtilityRate = overridenUtilityRate;
            stdRequest.OverridenUtilityBaseFee = overridenUtilityBaseRate;

            //as per new calculations
            stdRequest.SetAmountToFinanceReducer(0);

            var stdResponse = _loanCalculator.CalculateLoan(stdRequest, planDefinition);
            var stdPlan = stdResponse.ToPlanOption(SubPlan_Standard.ToUpper(), "Keep All Incentives", PlanOptionType.Standard);

            introMonthlyPayment = stdResponse.IntroMonthlyPayment;

            #endregion

            #region Smart Plan

            var smartRequest = param.FinancePlan.GetRequest(true);

            smartRequest.ScenarioTermInYears = scenarioTermInYears;
            smartRequest.IncludeMonthsInResponse = true;
            smartRequest.UtilityInflationRate = param.UtilityInflationRate ?? smartRequest.UtilityInflationRate;

            var smartIncentiveValues = GetIncentivesTotalForPlan(smartRequest, SubPlan_Smart);
            smartRequest.UpfrontRebate = smartIncentiveValues.Item1 + smartIncentiveValues.Item2;
            smartRequest.UpfrontRebateReducedFromITC = smartIncentiveValues.Item1;
            smartRequest.OverridenUtilityRate = overridenUtilityRate;
            smartRequest.OverridenUtilityBaseFee = overridenUtilityBaseRate;

            smartRequest.SetAmountToFinanceReducer(smartRequest.UpfrontRebate + smartRequest.FederalTaxIncentive + smartRequest.DownPayment);

            var smartResponse = _loanCalculator.CalculateLoan(smartRequest, planDefinition);
            var smartPlan = smartResponse.ToPlanOption(SubPlan_Smart.ToUpper(), "Apply FTC, State, Utility", PlanOptionType.Smart, introMonthlyPayment);

            #endregion

            #region Smarter plan
            // generate the 'Smarter' plan, apply ITC and rebate incentives.
            // adjust the finance plan settings to calculate the Smarter plan
            //var smarterRequest = financePlan.GetRequest(true);
            //smarterRequest.UtilityInflationRate = param.UtilityInflationRate ?? smarterRequest.UtilityInflationRate;
            //smarterRequest.ScenarioTermInYears = scenarioTermInYears;
            //smarterRequest.IncludeMonthsInResponse = true;

            //var smarterIncentiveValues = GetIncentivesTotalForPlan(stdRequest, SubPlan_Smarter);
            //smarterRequest.UpfrontRebate = smarterIncentiveValues.Item1 + smarterIncentiveValues.Item2;
            //smarterRequest.UpfrontRebateReducedFromITC = smarterIncentiveValues.Item1;

            //smarterRequest.SetAmountToFinanceReducer(smarterRequest.FederalTaxIncentive);

            //var smarterResponse = _loanCalculator.CalculateLoan(smarterRequest, planDefinition);
            var smarterRequest = financePlan.GetRequest(true);
            smarterRequest.OverridenUtilityRate = overridenUtilityRate;
            smarterRequest.OverridenUtilityBaseFee = overridenUtilityBaseRate;

            //var smarterResponse = CalculateOption(new OptionCalculatorModel
            //{
            //    Request = smarterRequest,
            //    PlanName = SubPlan_Smarter,
            //    PlanDefinition = planDefinition,
            //    ScenarioTermInYears = scenarioTermInYears,
            //    UtilityInflationRate = param.UtilityInflationRate
            //});


            //as per new calculation

            smarterRequest.UtilityInflationRate = param.UtilityInflationRate ?? smarterRequest.UtilityInflationRate;
            smarterRequest.ScenarioTermInYears = scenarioTermInYears;
            smarterRequest.IncludeMonthsInResponse = true;

            var smarterIncentiveValues = GetIncentivesTotalForPlan(smarterRequest, SubPlan_Smarter);
            smarterRequest.UpfrontRebate = smarterIncentiveValues.Item1 + smarterIncentiveValues.Item2;
            smarterRequest.UpfrontRebateReducedFromITC = smarterIncentiveValues.Item1;

            //smarterRequest.SetAmountToFinanceReducer(request.FederalTaxIncentive + request.UpfrontRebate);

            var year1 = smartResponse.Years[0]; 
            var monthlySavings = (year1.ElectricityBillWithoutSolar / 12) - (stdPlan.Payment18M + year1.ElectricityBillWithSolar / 12); 
            smarterRequest.SetAmountToFinanceReducer(smarterRequest.FederalTaxIncentive + smarterRequest.UpfrontRebate + (monthlySavings > 0 ? monthlySavings * 18 : 0));

            var smarterResponse = _loanCalculator.CalculateLoan(smarterRequest, planDefinition);

            var smarterPlan = smarterResponse.ToPlanOption(SubPlan_Smarter.ToUpper(), "Apply All Incentives and Savings", PlanOptionType.Smarter, introMonthlyPayment);

            proposal.FinancePlanOptions = new List<ProposalFinancePlanOption> { stdPlan, smartPlan, smarterPlan };
            //proposal.Financing.MonthlyPayment = (double)smarterPlan.Payment19M;
            proposal.Financing.MonthlyPayment = (double)smartPlan.Payment19M;  
            #endregion

            if (proposal.ForecastScenario != null)
            {
                var stdIncentives = GetIncentivesForPlan(stdRequest, SubPlan_Standard);
                var smartIncentives = GetIncentivesForPlan(stdRequest, SubPlan_Smart);
                smartIncentives = smartIncentives?
                                    .Where(si => !(stdIncentives?.Contains(si) == true))?
                                    .ToList();

                var smarterIncentives = GetIncentivesForPlanOnly(smarterRequest, SubPlan_Smarter);
                smarterIncentives = smarterIncentives?
                                    .Where(si => !(stdIncentives?.Contains(si) == true) && !(smartIncentives?.Contains(si) == true))
                                    .ToList();

                //proposal.ForecastScenario.TotalCost = (decimal)proposal.SystemCosts.Total;

                //proposal.ForecastScenario.StdRebates = stdIncentives?.ToDictionary(si => si.Name, si => si.GetGrandTotal(stdRequest.SystemSize));
                //proposal.ForecastScenario.SmartRebates = smartIncentives?.ToDictionary(si => si.Name, si => si.GetGrandTotal(smartRequest.SystemSize));
                //proposal.ForecastScenario.SmarterRebates = smarterIncentives?.ToDictionary(si => si.Name, si => si.GetGrandTotal(smarterRequest.SystemSize));

                proposal.ForecastScenario.StdRebates = stdIncentives.Select(a => new Incentive
                {
                    Guid = a.Guid,
                    Name = a.Name,
                    Cost = a.GetGrandTotal(smartRequest.SystemSize),
                    Quantity = a.Quantity,
                    IsRebate = a.IsRebate,
                    AllowsQuantitySelection = a.AllowsQuantitySelection,
                    AdderType = a.AdderType
                }).ToList();

                proposal.ForecastScenario.SmartRebates = smartIncentives.Select(a => new Incentive
                {
                    Guid = a.Guid,
                    Name = a.Name,
                    Cost = a.GetGrandTotal(smartRequest.SystemSize),
                    Quantity = a.Quantity,
                    IsRebate = a.IsRebate,
                    AllowsQuantitySelection = a.AllowsQuantitySelection
                }).ToList();

                proposal.ForecastScenario.SmarterRebates = smarterIncentives.Select(a => new Incentive
                {
                    Guid = a.Guid,
                    Name = a.Name,
                    Cost = a.GetGrandTotal(smartRequest.SystemSize),
                    Quantity = a.Quantity,
                    IsRebate = a.IsRebate,
                    AllowsQuantitySelection = a.AllowsQuantitySelection
                }).ToList(); ;

                // There are scenarios when there are incentives/rebates that apply before the ITC is calculated
                // to capture that, we'll use the smarter option for ITC (FederalTaxIncentive)

                //proposal.ForecastScenario.FedTaxCredit = smarterRequest.FederalTaxIncentive;

                if (financePlan.FinancePlanType == FinancePlanType.Cash)
                {
                    var smarterIncentivesTotal = smarterIncentives.Sum(i => i.GetGrandTotal(smarterRequest.SystemSize));
                    // for cash ee use the Smart Amount to finance as NetCost.
                    //  #133 old formulla - 02-04-2020  proposal.ForecastScenario.NetCost = stdRequest.GrossSystemCostWithTaxAndDealerFee - stdRequest.FederalTaxIncentive - smarterIncentivesTotal;
                    proposal.ForecastScenario.NetCost = stdRequest.GrossSystemCostWithAddersTaxAndDealearFee - stdRequest.FederalTaxIncentive - smarterIncentivesTotal;
                }
                else
                {
                    // We use the Smarter Amount to finance as NetCost.
                    proposal.ForecastScenario.NetCost = smarterRequest.AmountToFinance;
                }

                //as per new calculation 
                proposal.ForecastScenario.TotalCost = proposal.SystemCosts.TotalCostToCustomer;
                proposal.ForecastScenario.FedTaxCredit = proposal.SystemCosts.FederalTaxCredit;
                proposal.ForecastScenario.NetCost = proposal.SystemCosts.NetCost;
            }
        }

        public LoanResponse CalculateOption(OptionCalculatorModel args)
        {
            var request = args.Request;
            request.UtilityInflationRate = args.UtilityInflationRate ?? request.UtilityInflationRate;
            request.ScenarioTermInYears = args.ScenarioTermInYears;
            request.IncludeMonthsInResponse = true;

            var smarterIncentiveValues = GetIncentivesTotalForPlan(request, args.PlanName);
            request.UpfrontRebate = smarterIncentiveValues.Item1 + smarterIncentiveValues.Item2;
            request.UpfrontRebateReducedFromITC = smarterIncentiveValues.Item1;

            //request.SetAmountToFinanceReducer(request.FederalTaxIncentive + request.UpfrontRebate);
            //as per new calculation

            request.SetAmountToFinanceReducer(request.FederalTaxIncentive + request.UpfrontRebate);

            return _loanCalculator.CalculateLoan(request, args.PlanDefinition);
        }

        [Obsolete("This method is replaced by: AddLoanSummary.")]
        private void AddSpecialLoan(Proposal2DataView proposal, TriSmartConstructor param, FinanceDetail first)
        {
            var financePlan = param.FinancePlan;

            var scenarioTermInYears = 30;

            var smartRequest = param.FinancePlan.GetRequest(true);
            smartRequest.ScenarioTermInYears = scenarioTermInYears;
            smartRequest.IncludeMonthsInResponse = true;
            smartRequest.UtilityInflationRate = param.UtilityInflationRate ?? smartRequest.UtilityInflationRate;

            var financePlanDefinition = param.FinancePlan.FinancePlanDefinition;
            var planDetails = financePlanDefinition.Details.ToList();
            var proposalData = param.Data;


            first.ApplyReductionAfterPeriod = ReductionType.ReduceInTheBeginning;

            var smartResponse = _loanCalculator.CalculateLoan(smartRequest, financePlanDefinition);
            var smartPlan = smartResponse.ToPlanOption(SubPlan_Smart.ToUpper(), "Apply FTC, State, Utility", PlanOptionType.Smart, null, proposal.Financing.FinancingType);

            // generate the 'Standard' plan, don't apply incentives.
            // adjust the finance plan settings to calculate the Standard plan
            first.ApplyReductionAfterPeriod = ReductionType.ReduceInTheBeginningButAccumulateInterest;
            var stdRequest = financePlan.GetRequest(true);
            stdRequest.UtilityInflationRate = param.UtilityInflationRate ?? smartRequest.UtilityInflationRate;
            stdRequest.ScenarioTermInYears = scenarioTermInYears;
            stdRequest.IncludeMonthsInResponse = true;

            var stdResponse = _loanCalculator.CalculateLoan(stdRequest, financePlanDefinition);
            var stdPlan = stdResponse.ToPlanOption(SubPlan_Standard.ToUpper(), "Keep All Incentives", PlanOptionType.Standard, null, proposal.Financing.FinancingType);

            // generate the 'Smarter' plan, apply ITC and rebate incentives.
            // adjust the finance plan settings to calculate the Smarter plan
            var smarterRequest = financePlan.GetRequest(true);
            smarterRequest.UtilityInflationRate = param.UtilityInflationRate ?? smartRequest.UtilityInflationRate;
            smarterRequest.ScenarioTermInYears = scenarioTermInYears;
            smarterRequest.IncludeMonthsInResponse = true;

            first.ApplyReductionAfterPeriod = ReductionType.ReduceInTheBeginning;
            var nextDetailIndex = planDetails.IndexOf(first) + 1;
            var nextDetail = nextDetailIndex < planDetails.Count ? planDetails[nextDetailIndex] : null;
            if (nextDetail != null)
            {
                nextDetail.ApplyReductionAfterPeriod = ReductionType.ReduceInTheBeginningUsingRebatesIncentives;
                smarterRequest.RebateIncentives = param.Proposal
                                                        .SolarSystem
                                                        .AdderItems
                                                        .Where(a => a.IsRebate.HasValue && a.IsRebate.Value)
                                                        .Sum(a => a.CalculatedCost(param.Proposal.SolarSystem.SystemSize));
            }

            var smarterResponse = _loanCalculator.CalculateLoan(smarterRequest, financePlanDefinition);
            var smarterPlan = smarterResponse.ToPlanOption(SubPlan_Smarter.ToUpper(), "Apply All Incentives and Savings", PlanOptionType.Smarter, null, proposal.Financing.FinancingType);

            proposal.FinancePlanOptions = new List<ProposalFinancePlanOption> { stdPlan, smartPlan, smarterPlan };
        }

        private void AddSummary(Proposal2DataView proposal, TriSmartConstructor param)
        {
            var request = param.FinancePlan.GetRequest(true);
            request.UtilityInflationRate = param.UtilityInflationRate ?? request.UtilityInflationRate;
            request.ScenarioTermInYears = 30;
            request.IncludeMonthsInResponse = true;
            var financePlanDefinition = param.FinancePlan.FinancePlanDefinition;
            var response = _loanCalculator.CalculateLoan(request, financePlanDefinition);

            proposal.ForecastScenario = new ProposalForecast(response);
        }

        private List<Incentive> GetIncentivesForPlan(LoanRequest request, string plan)
        {
            return request?
                        .Incentives?
                        .Where(i => i.GetDynamicSettingsWithBoolValue(DynamicIncentive_Names.TriSMART_Incentives_SummaryOnly, true)? // applies to given SubPlan 
                                     .Any(ds => ds.Name == DynamicIncentive_Names.TriSMART_Incentives_SubPlans
                                            && ds.Type == AdderItemDynamicItemType.MultiSelect
                                            && ds.SelectedOptions?.Contains(plan, StringComparer.OrdinalIgnoreCase) == true) == true)?
                        .ToList();
        }

        /// <summary>
        /// Get all the incentives that are only ment for given plan (e.g. Advertising Bonus on TriSMART)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="plan"></param>
        /// <returns></returns>
        private List<Incentive> GetIncentivesForPlanOnly(LoanRequest request, string plan)
        {
            return request?
                        .Incentives?
                        .Where(i => i.GetDynamicSettingsWithBoolValue(DynamicIncentive_Names.TriSMART_Incentives_SummaryOnly, true)? // applies to given SubPlan 
                                     .Any(ds => ds.Name == DynamicIncentive_Names.TriSMART_Incentives_SubPlans
                                            && ds.Type == AdderItemDynamicItemType.MultiSelect
                                            && ds.SelectedOptions?.Count == 1
                                            && ds.SelectedOptions?.Contains(plan, StringComparer.OrdinalIgnoreCase) == true) == true)?
                        .ToList();
        }

        private List<Incentive> GetIncentivesForPlansOnly(LoanRequest request, List<string> plans)
        {
            return request?
                        .Incentives?
                        .Where(i => i.GetDynamicSettingsWithBoolValue(DynamicIncentive_Names.TriSMART_Incentives_SummaryOnly, true)? // applies to given SubPlan 
                                     .Any(ds => ds.Name == DynamicIncentive_Names.TriSMART_Incentives_SubPlans
                                            && ds.Type == AdderItemDynamicItemType.MultiSelect
                                            && ds.SelectedOptions?.Count == plans.Count
                                            && ds.SelectedOptions?.Intersect(plans)?.Count() == plans.Count) == true)?
                        .ToList();
        }

        private Tuple<decimal, decimal> GetIncentivesTotalForPlan(LoanRequest request, string plan)
        {
            var stdIncentives = GetIncentivesForPlan(request, plan);

            var preITCTotalIncentives = stdIncentives?
                                            .Where(i => i.IsAppliedBeforeITC)?
                                            .Sum(i => i.GetGrandTotal(request.SystemSize)) ?? 0;

            var postITCTotalIncentives = stdIncentives?
                                            .Where(i => !i.IsAppliedBeforeITC)?
                                            .Sum(i => i.GetGrandTotal(request.SystemSize)) ?? 0;

            return new Tuple<decimal, decimal>(preITCTotalIncentives, postITCTotalIncentives);
        }

        //private decimal GetIncentivesTotalForPlan(LoanRequest request, string plan)
        //{
        //    return GetIncentivesForPlan(request, plan)?.
        //                Sum(si => si.GetGrandTotal(request.SystemSize)) ?? 0;
        //}
    }
}
