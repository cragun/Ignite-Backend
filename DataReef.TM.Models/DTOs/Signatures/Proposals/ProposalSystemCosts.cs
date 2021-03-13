using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    public class ProposalSystemCosts
    {
        public List<SystemCostGroup> Groups { get; set; } = new List<SystemCostGroup>();

        public double TotalAdders { get; set; }
        public double TotalIncentives { get; set; }
        public double TotalRebates { get; set; }
        public double TotalSystem { get; set; }

        public double Total { get; set; }
        public double TotalCostToCustomer { get; set; }
        public double FederalTaxCredit { get; set; }


        public ProposalSystemCosts()
        {
        }

        public ProposalSystemCosts(FinancePlan financePlan, LoanRequest request, bool roundAmounts = false)
        {
            var solarSystem = financePlan?.SolarSystem;
            var panels = solarSystem?.GetPanels();
            var roofPlanes = solarSystem.RoofPlanes;
            var baseSystemGroup = new SystemCostGroup
            {
                GroupName = "Base System Cost",
                HasUnitPrice = false,
                Total = roundAmounts ? Math.Round((double)(request?.GrossSystemCostWithTaxAndDealerFee ?? 0)) : (double)(request?.GrossSystemCostWithTaxAndDealerFee ?? 0),
                Items = roofPlanes?
                            .GroupBy(rp => rp.SolarPanel)
                            .Select(rp => new SystemCostItem
                            {
                                Name = $"{rp.Key.Name} {rp.Key.Description} {rp.Key.Watts}w Panels",
                                Quantity = rp.Sum(panel => panel.PanelsCount)
                            })
                            .ToList()
            };
            Groups.Add(baseSystemGroup);

            var addersGroup = new SystemCostGroup
            {
                GroupName = "Additional Items",
                HasUnitPrice = true,
                Items = solarSystem?
                            .AdderItems?
                            .Where(a => a.Type == AdderItemType.Adder)
                            .Select(a => new SystemCostItem(a, solarSystem.SystemSize, false))
                .ToList()
            };
            addersGroup.CalculateTotal(roundAmounts);
            TotalAdders = addersGroup.Total ?? 0;
            Groups.Add(addersGroup);

            var incentivesGroup = new SystemCostGroup
            {
                GroupName = "Incentives",
                HasUnitPrice = true,
                ShouldSubtract = true,
                Items = solarSystem?
                            .AdderItems?
                            //.Where(a => a.Type == AdderItemType.Incentive && (!a.IsRebate.HasValue || (a.IsRebate.HasValue && !a.IsRebate.Value)))
                            .Where(a => a.IsProposalIncentive())
                            .Select(a => new SystemCostItem(a, solarSystem.SystemSize, false))
                .ToList()
            };
            incentivesGroup.CalculateTotal(roundAmounts);
            TotalIncentives = incentivesGroup.Total ?? 0;
            Groups.Add(incentivesGroup);

            var rebatesGroup = new SystemCostGroup
            {
                GroupName = "Rebates",
                HasUnitPrice = true,
                ShouldSubtract = true,
                UseWhenCalculatingTotal = false,
                Items = solarSystem?
                            .AdderItems?
                            //.Where(a => a.Type == AdderItemType.Incentive && a.IsRebate.HasValue && a.IsRebate.Value)
                            .Where(a => a.IsProposalIncentive(true))
                            .Select(a => new SystemCostItem(a, solarSystem.SystemSize, false))
                .ToList()
            };

            rebatesGroup.CalculateTotal(roundAmounts);
            TotalRebates = rebatesGroup.Total ?? 0;
            Groups.Add(rebatesGroup);

            Total = Groups
                        .Where(s => s.Total.HasValue && s.UseWhenCalculatingTotal)
                        .Sum(g => g.ShouldSubtract ? -g.Total.Value : g.Total.Value);

            TotalSystem = baseSystemGroup.Total ?? 0;

            TotalCostToCustomer = Math.Round(request?.TotalCostToCustomer ?? 0);
            FederalTaxCredit = Math.Round(request?.FederalTaxCredit ?? 0); 
        }
    }

    public class SystemCostItem
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public bool AllowsQuantitySelection { get; set; }
        
        public string UnitPriceDescription => RecurrenceType == AdderItemRecurrenceType.OneTime ? Label : $"{Label} x {RecurrencePeriod} {RecurrenceTypeLabel}";
        public double? UnitPriceValue { get; set; }
        [JsonIgnore]
        public AdderItemRateType RateType { get; set; }
        [JsonIgnore]
        public int SystemSize { get; set; }

        [JsonIgnore]
        public AdderItemRecurrenceType RecurrenceType { get; set; }
        [JsonIgnore]
        public int RecurrenceStart { get; set; }
        [JsonIgnore]
        public int RecurrencePeriod { get; set; }

        private double SystemSizeInKW => Math.Round(((double)SystemSize) / 1000, 2);

        public double Cost => RecurrenceType == AdderItemRecurrenceType.OneTime ? FlatCost : FlatCost * RecurrencePeriod;

        private double FlatCost => !UnitPriceValue.HasValue
                                ? 0
                                : RateType == AdderItemRateType.Flat
                                     ? Quantity * UnitPriceValue.Value
                                    : RateType == AdderItemRateType.PerKw
                                        ? SystemSizeInKW * Quantity * UnitPriceValue.Value
                                          : SystemSize * Quantity * UnitPriceValue.Value;

        private string Label => !UnitPriceValue.HasValue ? null : RateType == AdderItemRateType.Flat
            ? $"${UnitPriceValue.Value:0.##}"
            : RateType == AdderItemRateType.PerKw
                ? $"${UnitPriceValue.Value:0.##} x {SystemSizeInKW:0.##} kW"
                : $"${UnitPriceValue.Value:0.##} x {SystemSize:0.##} Watts";

        private string RecurrencePlural => RecurrencePeriod > 1 ? "s" : "";

        private string RecurrenceTypeLabel => RecurrenceType == AdderItemRecurrenceType.Monthly ? $"Month{RecurrencePlural}" : $"Year{RecurrencePlural}";

        public SystemCostItem()
        { }

        /// <summary>
        /// Create a SystemCostItem using data from an AdderItem
        /// </summary>
        /// <param name="adder"></param>
        /// <param name="systemSize">System Size in Watts</param>
        /// <param name="roundAmounts"></param>
        public SystemCostItem(AdderItem adder, int systemSize, bool roundAmounts = false)
        {
            Name = adder.Name;
            Guid = adder.Guid;
            Description = adder.ReductionDescription();
            Quantity = adder.Quantity;
            UnitPriceValue = roundAmounts ? Math.Round((double)adder.Cost) : (double)adder.Cost;
            RateType = adder.RateType;
            SystemSize = systemSize;
            AllowsQuantitySelection = Convert.ToBoolean(adder.AllowsQuantitySelection);
            RecurrenceType = adder.RecurrenceType;
            RecurrenceStart = adder.RecurrenceStart;
            RecurrencePeriod = adder.RecurrencePeriod;
        }
    }

    public class SystemCostGroup
    {
        public string GroupName { get; set; }
        public List<SystemCostItem> Items { get; set; }
        public bool HasUnitPrice { get; set; }
        public double? Total { get; set; }
        [JsonIgnore]
        public bool ShouldSubtract { get; set; }
        [JsonIgnore]
        public bool UseWhenCalculatingTotal { get; set; } = true;

        public void CalculateTotal(bool roundAmounts = false)
        {
            Total = Items
                    .Where(i => i.UnitPriceValue.HasValue)
                    .Sum(i => i.Cost);

            if(roundAmounts)
            {
                Total = Math.Round((double)Total);
            }
        }
    }
}