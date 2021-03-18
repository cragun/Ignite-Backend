using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class LoanRequest
    {
        /// <summary>
        /// Duration of the scenario in years (including all the years of the financing period)
        /// </summary>
        public int ScenarioTermInYears { get; set; }

        public List<EnergyMonthDetails> MonthlyPower { get; set; }

        public MortgageAnalysisRequest MortgageData { get; set; }

        /// <summary>
        /// $/W
        /// </summary>
        public decimal PricePerWattASP { get; set; }

        /// <summary>
        /// List of RoofPlane Production and Price per Watt for that roofplane
        /// </summary>
        public List<ProductionPrice> ProductionCosts { get; set; }

        public long SystemSize { get; set; }

        public decimal TaxRate { get; set; }

        public decimal DownPayment { get; set; }

        /// <summary>
        /// Percentge
        /// </summary>
        public double UtilityInflationRate { get; set; }

        /// <summary>
        /// annual system degredation rate
        /// </summary>
        public double Derate { get; set; }


        #region Adders
        /// TODO: to be renamed to TotalAddersCosts (also in response, update JSON in db with new name)
        /// <summary>
        /// Adders costs besides the solar system implicit costs (e.g. Cost for replacing the roof, cutting trees, extra light bulbs, etc.)
        /// Adders costs supported by the home owner only
        /// </summary>
        public decimal TotalAddersCosts { get; set; }

        /// <summary>
        /// Adders costs supported by the sales rep
        /// </summary>
        public decimal AddersPaidByRep { get; set; }

        /// <summary>
        /// List of adders
        /// </summary>
        public List<AdderItem> Adders { get; set; }

        public decimal TotalAddersCostsWithFinancingFee
        {
            get
            {
                return
                    Adders
                    ?.Where(a => a.Type == AdderItemType.Adder)
                    ?.Sum(a => a.CalculatedCost(SystemSize, DealerFee, true)) ?? TotalAddersCosts;
            }
        }


        public decimal TotalAddersCostsWithOutFinancingFee
        {
            get
            {
                return
                    Adders
                    ?.Where(a => a.Type == AdderItemType.Adder)
                    ?.Sum(a => a.CalculatedCost(SystemSize, DealerFee, false)) ?? TotalAddersCosts;
            }
        }

        public decimal TotalAddersBeforeITCWithFinancingFee
        {
            get
            {
                return
                    Adders
                    ?.Where(a => a.Type == AdderItemType.Adder && a.IsAppliedBeforeITC)
                    ?.Sum(a => a.CalculatedCost(SystemSize, DealerFee, true)) ?? 0;
            }
        }
        #endregion

        #region BenefitsAndIncentives


        /// <summary>
        /// The percentage of the AmountFinanced that represents the federal tax incentive. An usual value is 30% = 0.3. This incentive is applied at the end of the IntroTermInMonths.
        /// </summary>
        public decimal FederalTaxIncentivePercentage { get; set; }

        /// <summary>
        /// The amount of money that are being received from the states that offer this incentive. This value is directly in dollars. This incentive is applied at the end of the into loan, just like the ITC.
        /// </summary>
        public decimal StateTaxIncentive { get; set; }

        /// <summary>
        /// Performance-based incentives. Usual value: 0.02 $/kWh for a period of 2 years. This incentive is applied monthly starting with the second month.
        /// </summary>
        public decimal PBI { get; set; }

        public int PBITermInYears { get; set; }

        /// <summary>
        /// Solar Renewable Energy Credit. Usual value: 0.05 $/kWh for a period of 2 years. This incentive is applied yearly starting with the first month of the second year.
        /// </summary>
        public decimal SREC { get; set; }

        public int SRECTermInYears { get; set; }

        /// <summary>
        /// You sell the rights to all of the SRECs that will ever be produced by your system. You get a single, lump sum payment upfront. After that you receive no money for SRECs for the lifetime of your system.
        /// </summary>
        public decimal UpfrontRebate { get; set; }

        /// <summary>
        /// The upfront incentive / rebate amount reduced from the Federal Investment Tax Credit
        /// </summary>
        public decimal UpfrontRebateReducedFromITC { get; set; }

        //  The total incentives (of the ones based on the adders model
        public decimal TotalIncentives { get; set; }

        /// <summary>
        /// Rebate incentives. Usually are not reduced from system cost.
        /// </summary>
        public decimal RebateIncentives { get; set; }

        public List<DealerIncentive> DealerIncentives { get; set; }

        public ICollection<Incentive> Incentives { get; set; }

        public bool ApplyITCToLoan { get; set; } = true;

        #endregion BenefitsAndIncentives

        #region Lease Properties

        /// <summary>
        /// Lease price per kWh
        /// </summary>
        public decimal LeasePricePerkWh { get; set; }

        /// <summary>
        /// Annual escalator percentage
        /// </summary>
        public decimal Escalator { get; set; }

        /// <summary>
        /// Monthly lease tax percentage. Will be added to monthly lease payment.
        /// </summary>
        public decimal MonthlyLeaseTax { get; set; }

        /// <summary>
        /// Lease Parameters
        /// </summary>
        public LeaseParameters LeaseParams { get; set; }

        #endregion


        #region computed properties and methods

        public List<EnergyMonthDetails> OrderedMonthPower => MonthlyPower?.OrderBy(mp => mp.Month)?.ToList();

        public decimal GrossSystemCost
        {
            get
            {
                if (ProductionCosts != null && ProductionCosts.Any(pc => pc.PricePerWatt != 0 && pc.Production != 0))
                {
                    return ProductionCosts.Sum(pc => pc.Production * pc.PricePerWatt);
                }
                return SystemSize * PricePerWattASP;
            }
        }
        public decimal GrossSystemCostWithTax => GrossSystemCost * (1 + TaxRate);

        /// <summary>
        /// First year production multiplied by LeaseParams.ProductionIncrease percentage
        /// </summary>
        public decimal LeaseProduction => FirstYearElectricityProduction * (((LeaseParams?.ProductionIncrease ?? 0) / 100) + 1);

        public decimal LeaseSystemCost => LeaseProduction * (LeaseParams?.LeasePricePerkWh ?? LeasePricePerkWh);

        public decimal LeaseSystemCostWithTax => LeaseSystemCost * (1 + TaxRate);

        public decimal LeaseSystemCostWithExtrasAndTax => LeaseSystemCostWithTax + ExtraCostsWithTax;

        public decimal FederalTaxIncentive => (GrossSystemCostWithAddersTaxAndDealearFee - UpfrontRebateReducedFromITC) * FederalTaxIncentivePercentage;

        /// <summary>
        /// Gross SystemCost with tax And Dealer Fee
        /// </summary>
        public decimal GrossSystemCostWithTaxAndDealerFee => GrossSystemCostWithTax + DealerFeeCost;

        public decimal GrossSystemCostWithAddersTaxAndDealearFee
        {
            get
            {
                var sum = GrossSystemCostWithTax + TotalAddersBeforeITCWithFinancingFee;
                if (DealerFee > 0)
                {
                    sum = sum * (DealerFee / 100);
                }

                return sum;
            }
        }

        public decimal ExtraCostsWithTax => (TotalAddersCostsWithFinancingFee - AddersPaidByRep) * (1 + TaxRate);

        //public decimal AmountToFinance => Math.Max(Math.Round((GrossSystemCostWithTaxAndDealerFee + ExtraCostsWithTax + (IncludeAmountToRefinance ? AmountToRefinance : 0)) - DownPayment - UpfrontRebate - AmountToFinanceReducer, 2), 0);

        public decimal AmountToFinance => (decimal)TotalCostToCustomer;

        public decimal AmountToFinanceUnreduced => AmountToFinance + AmountToFinanceReducer;

        public decimal AmountToRefinance => MortgageData?.CurrentMortgage?.CurrentBalance + MortgageData?.ClosingCostRate ?? 0;

        public decimal FirstYearElectricityCosts => MonthlyPower?.Sum(mp => mp.PreSolarCost) ?? 0;
        public decimal FirstYearElectricityConsumption => MonthlyPower?.Sum(mp => mp.PostAddersConsumptionOrConsumption) ?? 0;

        public decimal FirstYearElectricityProduction => MonthlyPower?.Sum(mp => mp.Production) ?? 0;

        public decimal DealerFeeCost
        {
            get
            {
                if (DealerFee <= 0)
                {
                    return 0;
                }

                return GrossSystemCost * (DealerFee / 100);
            }
        }

        public decimal LeaseEscalator => LeaseParams?.Escalator ?? Escalator;

        #endregion

        #region optional parameters

        /// <summary>
        /// This property is only used when calculating the Mortgage for the Entire amount (solar + refinance)
        /// Only true when calculating entire mortgage (solar + refinance)
        /// </summary>
        [JsonIgnore]
        public bool IncludeAmountToRefinance { get; set; }

        /// <summary>
        /// If true, the months will be included in the response
        /// </summary>
        public bool IncludeMonthsInResponse { get; set; }

        /// <summary>
        /// DealerFee Percentage that needs to get backed into calculations
        /// On ComparisonRequest, this value will be: 1: enabled / 0: disabled
        /// </summary>
        public decimal DealerFee { get; set; }

        /// <summary>
        /// Finance Plan specific Additional data as JSON
        /// </summary>
        public string FinancePlanData { get; set; }

        /// <summary>
        /// This property will be used as part of electricity bill with solar calculation
        /// </summary>
        public decimal? OverridenUtilityRate { get; set; }

        /// <summary>
        /// Utility Base Fee per Month - amount paid by the customer as a base utility fee.
        /// </summary>
        public decimal? OverridenUtilityBaseFee { get; set; }

        #endregion optional parameters

        #region Methods

        public decimal GetDealerIncentivesForMonth(int monthIndex)
        {
            return DealerIncentives?
                        .Select(di => di.GetIncentiveValue(monthIndex, SystemSize))
                        .Sum() ?? 0;
        }

        public decimal TotalDealerIncentives()
        {
            return DealerIncentives?
                        .Select(di => di.GetGrandTotal(SystemSize))
                        .Sum() ?? 0;
        }

        public void SetAmountToFinanceReducer(decimal value)
        {
            AmountToFinanceReducer = value;
        }

        public T GetPlanData<T>()
        {
            if (string.IsNullOrWhiteSpace(FinancePlanData))
            {
                return default(T);
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(FinancePlanData);
            }
            catch { }

            return default(T);
        }

        #endregion

        #region Private properties

        /// <summary>
        /// Property used to decrease the amount to finance.
        /// There are scenarios, like LoanPal, when we want to simulate Smart / Smarter plan, and we need to reduce the amount to finance.
        /// </summary>
        private decimal AmountToFinanceReducer { get; set; }

        #endregion

        #region New Calculation 

        public double BaseLoanAmount
        {
            get
            {
                return Convert.ToDouble(((PricePerWattASP * SystemSize) + TotalAddersCostsWithOutFinancingFee) - UpfrontRebate - DownPayment);
            }
        }

        public double FinalLoanAmount
        {
            get
            {
                if (BaseLoanAmount != 0)
                {
                    return Math.Round(BaseLoanAmount * (BaseLoanAmount / (BaseLoanAmount * (1 - (Convert.ToDouble((DealerFee)) / 100)))), 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        public double TotalCostToCustomer
        {
            get
            {
                return FinalLoanAmount + Convert.ToDouble(DownPayment);
            }
        }

        public double FederalTaxCredit
        {
            get
            {
                return TotalCostToCustomer * (double)FederalTaxIncentivePercentage;
            }
        } 

        #endregion
    }


}
