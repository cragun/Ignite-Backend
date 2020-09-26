using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models.Solar;
using System;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class AdderDataView
    {
        public string Name { get; set; }

        public Guid AdderId { get; set; }

        public string AdderType { get; set; }

        public decimal? Cost { get; set; }

        public string RateType { get; set; }

        public double? Quantity { get; set; }

        public bool ReducesConsumption { get; set; }

        public decimal? ConsumptionReductionAmount { get; set; }

        public string ConsumptionReductionType { get; set; }

        public bool AddToSystemCost { get; set; }

        public bool IsCalculatedPerRoofPlane { get; set; }

        public Guid TemplateID { get; set; }

        public bool? IsPaidBySalesPerson { get; set; }
        public bool? IsRebate { get; set; }
        public bool IsAppliedBeforeITC { get; set; }
        public bool? AllowsQuantitySelection { get; set; }
        public bool IsSolarThermal { get; set; }
        public string DynamicSettingsJSON { get; set; }
        public bool? ApplyDealerFee { get; set; }


        public string Description { get; set; }

        public decimal? FinancingFee { get; set; }

        public decimal TotalReducedAmount => (ConsumptionReductionAmount ?? 0 * (decimal)(Quantity ?? 0));

        public decimal TotalCost => (Cost ?? 0) * (decimal)(Quantity ?? 0);


        public static AdderDataView FromDbModel(AdderItem item)
        {
            AdderDataView ret = new Signatures.AdderDataView();
            ret.Name = item.Name;
            ret.AdderId = item.Guid;
            ret.AdderType = Enum.GetName(typeof(Enums.AdderItemType), item.Type);
            ret.AddToSystemCost = item.AddToSystemCost;
            ret.ConsumptionReductionAmount = item.UsageReductionAmount;
            ret.ConsumptionReductionType = Enum.GetName(typeof(Enums.AdderItemReducedAmountType), item.UsageReductionType);
            ret.Cost = item.Cost;
            ret.Description = item.Description;
            ret.IsCalculatedPerRoofPlane = item.GetIsCalculatedPerRoofPlane();
            ret.IsPaidBySalesPerson = item.CanBePaidForByRep;
            ret.Quantity = item.Quantity;
            ret.RateType = Enum.GetName(typeof(Enums.AdderItemRateType), item.RateType);
            ret.ReducesConsumption = item.ReducesUsage;
            ret.TemplateID = item.TemplateID;
            ret.FinancingFee = item.FinancingFee;

            return ret;
        }

        public static AdderItem ToDbModel(AdderDataView item, Guid solarSystemID)
        {
            AdderItem ret = new Models.Solar.AdderItem();
            ret.Name = item.Name;
            ret.Guid = item.AdderId == Guid.Empty ? Guid.NewGuid() : item.AdderId;
            ret.Type = (Enums.AdderItemType)Enum.Parse(typeof(Enums.AdderItemType), item.AdderType);
            ret.AddToSystemCost = item.AddToSystemCost;
            ret.UsageReductionAmount = item.ConsumptionReductionAmount;
            ret.UsageReductionType = (Enums.AdderItemReducedAmountType)Enum.Parse(typeof(Enums.AdderItemReducedAmountType), item.ConsumptionReductionType);
            ret.Cost = item.Cost ?? 0;
            ret.Description = item.Description;
            ret.IsCalculatedPerRoofPlane = item.IsCalculatedPerRoofPlane;
            ret.CanBePaidForByRep = item.IsPaidBySalesPerson;
            ret.Quantity = item.Quantity ?? 1;
            ret.RateType = (Enums.AdderItemRateType)Enum.Parse(typeof(Enums.AdderItemRateType), item.RateType);
            ret.ReducesUsage = item.ReducesConsumption;
            ret.TemplateID = item.TemplateID;
            ret.Version = 1;
            ret.CreatedByID = SmartPrincipal.UserId;
            ret.CreatedByName = "Api User";
            ret.DateCreated = System.DateTime.UtcNow;
            ret.DateLastModified = System.DateTime.UtcNow;
            ret.IsDeleted = false;
            ret.LastModifiedBy = ret.CreatedByID;
            ret.LastModifiedByName = ret.CreatedByName;
            ret.SolarSystemID = solarSystemID;
            ret.TenantID = 0;
            ret.FinancingFee = item.FinancingFee;
            ret.IsRebate = item.IsRebate;
            ret.IsAppliedBeforeITC = item.IsAppliedBeforeITC;
            ret.AllowsQuantitySelection = item.AllowsQuantitySelection;
            ret.IsSolarThermal = item.IsSolarThermal;
            ret.DynamicSettingsJSON = item.DynamicSettingsJSON;
            ret.ApplyDealerFee = item.ApplyDealerFee;

            return ret;
        }


    }
}
