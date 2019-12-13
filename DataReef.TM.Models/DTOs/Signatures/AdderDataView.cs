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


        public AdderItem ToDbModel(Guid solarSystemID)
        {
            AdderItem ret = new Models.Solar.AdderItem();
            ret.Name = this.Name;
            ret.Guid = this.AdderId == Guid.Empty ? Guid.NewGuid() : this.AdderId;
            ret.Type = (Enums.AdderItemType)Enum.Parse(typeof(Enums.AdderItemType), this.AdderType);
            ret.AddToSystemCost = this.AddToSystemCost;
            ret.UsageReductionAmount = this.ConsumptionReductionAmount;
            ret.UsageReductionType = (Enums.AdderItemReducedAmountType)Enum.Parse(typeof(Enums.AdderItemReducedAmountType), this.ConsumptionReductionType);
            ret.Cost = this.Cost ?? 0;
            ret.Description = this.Description;
            ret.IsCalculatedPerRoofPlane = this.IsCalculatedPerRoofPlane;
            ret.CanBePaidForByRep = this.IsPaidBySalesPerson;
            ret.Quantity = this.Quantity ?? 1;
            ret.RateType = (Enums.AdderItemRateType)Enum.Parse(typeof(Enums.AdderItemRateType), this.RateType);
            ret.ReducesUsage = this.ReducesConsumption;
            ret.TemplateID = this.TemplateID;
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
            ret.FinancingFee = this.FinancingFee;            

            return ret;
        }


    }
}
