using DataReef.Core.Extensions;
using DataReef.TM.Models.DataViews.Financing;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.DTOs.Signatures.Proposals;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard
{
    public class SBProposalDataModel
    {
        public SBProposalDataModel() { }

        public SBProposalDataModel(Proposal proposal, FinancePlan financePlan, Proposal2DataView proposalDataView)
        {
            //proposal.SolarSystem.RoofPlanes.FirstOrDefault().SolarPanel
            //proposal.SolarSystem.RoofPlanes.FirstOrDefault().Inverter
            //proposal.SolarSystem.AdderItems
            //proposal.SolarSystem.SystemProduction.Months
            //financePlan.FinancePlanDefinition            

            if (proposal?.SolarSystem == null)
            {
                return;
            }

            var solarPanel = proposal.SolarSystem.GetSolarPanel();
            var inverter = proposal.SolarSystem.GetInverter();
            var loanRequest = financePlan?.Request;
            var loanResponse = financePlan?.Response;

            ModuleCount = proposal.SolarSystem.PanelCount;
            ModuleMake = solarPanel?.Name;
            ModuleModel = solarPanel?.Description;
            ModuleSize = solarPanel?.Watts;

            ProductionKWH = proposal.ProductionKWH;
            ProductionKWHpercentage = proposal.ProductionKWHpercentage;
            IsManual = proposal.IsManual; 

            InverterMake = inverter?.Name;
            InverterModel = inverter?.Model;
            InverterQuantity = proposal.SolarSystem.GetInvertersCount();

            SystemSize = (double)proposal.SolarSystem.SystemSize / 1000;

            TotalArrays = proposal
                            .SolarSystem?
                            .RoofPlanes?
                            .Where(rp => rp.PanelsCount > 0)?
                            .Count();
            ArraysData = proposal
                            .SolarSystem?
                            .RoofPlanes?
                            .Select(rp => new SBProposalArrayData
                            {
                                Quantity = rp.PanelsCount,
                                Azimuth = rp.Azimuth,
                                Tilt = rp.Tilt,
                                Shading = rp.Shading,
                                Production = rp.Production(),
                                Pitch = rp.Pitch
                            })?
                            .ToList();

            AddersData = proposal
                            .SolarSystem
                            .AdderItems?
                            .Where(ai => ai.Type == AdderItemType.Adder)?
                            .Select(a => new SBProposalAdderData
                            {
                                Name = a.Name,
                                Cost = a.Cost,
                                Quantity = a.Quantity,
                                RateType = a.RateType,
                                IsAppliedBeforeITC = a.IsAppliedBeforeITC,
                                DynamicData = a.GetDynamicSettings(),
                                ReducesUsage = a.ReducesUsage,
                                AddToSystemCost = a.AddToSystemCost,
                                IsCalculatedPerRoofPlane = a.GetIsCalculatedPerRoofPlane(),
                                UsageReductionAmount = a.UsageReductionAmount,
                                UsageReductionType = a.UsageReductionType,
                                TotalCost = a.CalculatedCost(proposal.SolarSystem.SystemSize)
                            })?
                            .ToList();


            IncentivesData = proposal
                            .SolarSystem
                            .AdderItems?
                            .Where(ai => ai.Type == AdderItemType.Incentive)?
                            .Select(a => new SBProposalIncentivesData
                            {
                                Name = a.Name,
                                Cost = a.Cost,
                                Quantity = a.Quantity,
                                RateType = a.RateType,
                                IsAppliedBeforeITC = a.IsAppliedBeforeITC,
                                DynamicData = a.GetDynamicSettings(),
                                IsCalculatedPerRoofPlane = a.GetIsCalculatedPerRoofPlane(),
                                TotalCost = a.CalculatedCost(proposal.SolarSystem.SystemSize)
                            })?
                            .ToList();

            MonthlyProduction = proposal
                            .SolarSystem?
                            .SystemProduction?
                            .Months?
                            .Select(m => new SBProposalMonthData
                            {
                                Month = m.Month,
                                Production = m.Production,
                                Consumption = m.Consumption,
                            })?
                            .ToList();

            Y1SolarProduction = proposal
                            .SolarSystem?
                            .SystemProduction?
                            .Production;
            UsageOffset = proposal
                            .SolarSystem?
                            .SystemProduction?
                            .UsageOffset();

            var financeMeta = financePlan?.FinancePlanDefinition?.GetMetaData<FinancePlanDataModel>();

            FinanceType = financePlan?.FinancePlanType.ToString();
            Lender = financePlan?.FinancePlanDefinition?.Provider?.Name;
            //LenderFee = financePlan?.FinancePlanDefinition?.LenderFee;
            LenderFee = financePlan?.FinancePlanDefinition?.DealerFee;
            LenderID = financeMeta?.SBMeta?.LenderID;
            LeasePricePerKWH = loanRequest?.LeaseParams?.PricePerkWh;
            LeaseEscalator = loanRequest?.LeaseParams?.Escalator;
            // FinanceTerm = financePlan?.FinancePlanDefinition?.GetTermInMonths();
            FinanceTerm = financePlan.FinancePlanDefinition?.TermInYears ?? 0;
            DealerFee = loanRequest?.DealerFee;
            FinanceAPR = financePlan?.FinancePlanDefinition?.Apr;
            FinanceLabel = financePlan?.FinancePlanDefinition?.Name;
            //InitialLoanAmount = loanResponse?.AmountFinanced;

            //TotalCost = loanResponse?.SolarSystemCost;

            // as per new calculation
            TotalCost = loanRequest?.TotalCostToCustomer;
            //PricePerWatt = loanRequest?.FinalPricePerWatt;
            PricePerWatt = 3; 
            InitialLoanAmount = loanRequest?.AmountToFinance;
            FedTaxCredit = loanRequest?.FederalTaxCredit; 

            //FedTaxCredit = loanResponse?.TotalFederalTaxIncentive;
             

            var stdPlan = proposalDataView?
                                    .FinancePlanOptions?
                                    .FirstOrDefault(fpo => fpo.PlanOptionType == PlanOptionType.Standard);

            var smartPlan = proposalDataView?
                                    .FinancePlanOptions?
                                    .FirstOrDefault(fpo => fpo.PlanOptionType == PlanOptionType.Smart);

            var smarterPlan = proposalDataView?
                                    .FinancePlanOptions?
                                    .FirstOrDefault(fpo => fpo.PlanOptionType == PlanOptionType.Smarter);
            FirstPeriodMonths = stdPlan?.PaymentFactorsFirstPeriod;

            StandardFirstPeriodPayment = StandardPayment1_18 = stdPlan?.Payment18M;

            StandardSecondPeriodPayment = StandardPayment_19_End = stdPlan?.Payment19M;
            SmartSecondPeriodPayment = SmartPayment_19_End = smartPlan?.Payment19M;
            SmarterSecondPeriodPayment = SmarterPayment_19_End = smarterPlan?.Payment19M;

            OldUtilityBill = proposalDataView?.EnergyCosts?.WithoutSolar?.MonthlyAverage;
            EstimatedNewUtilityBill = proposalDataView?.EnergyCosts?.WithSolar?.MonthlyAverage;
            NetMonthlySavings = proposalDataView?.EnergyCosts?.Savings?.MonthlyAverage;
            UsageCollected = proposalDataView?.UsageCollected;
            HoaName = proposal.Property?.PropertyBag?.FirstOrDefault(x => x.DisplayName == "HOA/Management Name")?.Value;
            HoaPhoneEmail = proposal.Property?.PropertyBag?.FirstOrDefault(x => x.DisplayName == "HOA/Management Phone/Email")?.Value;
        }

        #region Modules properties

        public int ModuleCount { get; set; }
        public string ModuleMake { get; set; }
        public string ModuleModel { get; set; }
        public int? ModuleSize { get; set; }

        #endregion

        #region Manual Production
        public double ProductionKWH { get; set; }
        public double ProductionKWHpercentage { get; set; }
        public bool IsManual { get; set; }

        #endregion

        #region Inverter properties

        public string InverterMake { get; set; }
        public string InverterModel { get; set; }
        public int InverterQuantity { get; set; }

        #endregion

        /// <summary>
        /// In kW
        /// </summary>
        public double SystemSize { get; set; }

        public List<SBProposalMonthData> MonthlyProduction { get; set; }

        /// <summary>
        /// In kWh
        /// </summary>
        public double? Y1SolarProduction { get; set; }

        /// <summary>
        /// Percentage
        /// </summary>
        public double? UsageOffset { get; set; }

        public decimal? PricePerWatt { get; set; }
        public decimal? TotalCost { get; set; }

        public decimal? FedTaxCredit { get; set; }
        public decimal? UtilityRebate { get; set; }
        public decimal? AdvertisingBonus { get; set; }
        public decimal? LeasePricePerKWH { get; set; }
        public decimal? LeaseEscalator { get; set; }
        public string Lender { get; set; }
        public string LenderID { get; set; }
        public string FinanceType { get; set; }
        public int? FinanceTerm { get; set; }
        public double? FinanceAPR { get; set; }
        public double? LenderFee { get; set; }
        public decimal? DealerFee { get; set; }
        public string FinanceLabel { get; set; }
        public decimal? InitialLoanAmount { get; set; }
        public decimal? StandardPayment1_18 { get; set; }
        public decimal? StandardPayment_19_End { get; set; }
        public decimal? SmartPayment_19_End { get; set; }
        public decimal? SmarterPayment_19_End { get; set; }
        public double? OldUtilityBill { get; set; }
        public double? EstimatedNewUtilityBill { get; set; }
        public double? NetMonthlySavings { get; set; }

        public int? FirstPeriodMonths { get; set; }
        public decimal? StandardFirstPeriodPayment { get; set; }
        public decimal? StandardSecondPeriodPayment { get; set; }
        public decimal? SmartSecondPeriodPayment { get; set; }
        public decimal? SmarterSecondPeriodPayment { get; set; }

        public int? TotalArrays { get; set; }

        public List<SBProposalArrayData> ArraysData { get; set; }

        public List<SBProposalAdderData> AddersData { get; set; }

        public List<SBProposalIncentivesData> IncentivesData { get; set; }

        public string HoaName { get; set; }

        public string HoaPhoneEmail { get; set; }

        public bool? UsageCollected { get; set; }
    }

    public class SBProposalMonthData
    {
        public int Month { get; set; }

        /// <summary>
        /// In kWh
        /// </summary>
        public float Production { get; set; }

        public float Consumption { get; set; }
    }

    public class SBProposalArrayData
    {
        public int Quantity { get; set; }
        public double Tilt { get; set; }
        public int Azimuth { get; set; }
        public int Production { get; set; }
        public int Shading { get; set; }
        public double Pitch { get; set; }
    }

    public class SBProposalBaseAdderIncentive
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public decimal Cost { get; set; }
        public AdderItemRateType RateType { get; set; }
        public bool IsAppliedBeforeITC { get; set; }
        public List<AdderItemDynamicUserDataDataView> DynamicData { get; set; }

        public bool IsCalculatedPerRoofPlane { get; set; }

        public decimal TotalCost { get; set; }
    }

    public class SBProposalAdderData : SBProposalBaseAdderIncentive
    {
        public bool ReducesUsage { get; set; }
        public decimal? UsageReductionAmount { get; set; }
        public AdderItemReducedAmountType? UsageReductionType { get; set; }
        public bool AddToSystemCost { get; set; }
    }

    public class SBProposalIncentivesData : SBProposalBaseAdderIncentive
    {
    }

}
