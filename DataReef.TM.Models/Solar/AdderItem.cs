using DataReef.Core;
using DataReef.Core.Attributes;
using DataReef.Core.Other;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("AdderItems", Schema = "solar")]
    public class AdderItem : EntityBase
    {
        [DataMember]
        public AdderItemType Type { get; set; }

        [DataMember]
        [JsonConverter(typeof(EmptyStringToJsonDeserializer))]
        public Guid SolarSystemID { get; set; }

        [DataMember]
        public decimal Cost { get; set; }

        [DataMember]
        public AdderItemRateType RateType { get; set; }

        [DataMember]
        public double Quantity { get; set; }

        [DataMember]
        public bool ReducesUsage { get; set; }

        [DataMember]
        public decimal? UsageReductionAmount { get; set; }

        [DataMember]
        public AdderItemReducedAmountType? UsageReductionType { get; set; }

        [DataMember]
        public bool AddToSystemCost { get; set; }

        [DataMember]
        public bool IsCalculatedPerRoofPlane { get; set; }

        [DataMember]
        public AdderItemCalculatedPerType? CalculatedPer { get; set; }

        [DataMember]
        public Guid TemplateID { get; set; }

        [DataMember]
        public bool? AllowsQuantitySelection { get; set; }

        [DataMember]
        public bool? CanBePaidForByRep { get; set; }

        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Is the adder the line item for the solar thermal panels
        /// </summary>
        [DataMember]
        public bool IsSolarThermal { get; set; }

        [DataMember]
        public bool? IsRebate { get; set; }

        [DataMember]
        public bool IsAppliedBeforeITC { get; set; }

        [DataMember]
        public AdderItemRecurrenceType RecurrenceType { get; set; }

        /// <summary>
        /// Month or Year start index.
        /// If it starts in first year, it would be 1
        /// </summary>
        [DataMember]
        public int RecurrenceStart { get; set; }

        /// <summary>
        /// Number of months or years to apply the Incentive
        /// </summary>
        [DataMember]
        public int RecurrencePeriod { get; set; }

        /// <summary>
        /// JSON serialized custom settings selected by the user.
        /// </summary>
        [DataMember]
        public string DynamicSettingsJSON { get; set; }

        [DataMember]
        public decimal? FinancingFee { get; set; }

        [DataMember]
        public bool? ApplyDealerFee { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey(nameof(SolarSystemID))]
        public SolarSystem SolarSystem { get; set; }

        [DataMember]
        [InverseProperty(nameof(RoofPlaneDetail.AdderItem))]
        [AttachOnUpdate]
        public ICollection<RoofPlaneDetail> RoofPlaneDetails { get; set; }

        #endregion

        [NotMapped]
        [JsonIgnore]
        public decimal TotalReducedAmount => UsageReductionAmount * (decimal)Quantity ?? 0;

        [NotMapped]
        [JsonIgnore]
        public decimal TotalCost => Cost * (decimal)Quantity;

        public AdderItem Clone(Guid solarSystemID, CloneSettings cloneSettings)
        {
            AdderItem ret = (AdderItem)this.MemberwiseClone();
            ret.Reset();
            ret.SolarSystem = null;
            ret.SolarSystemID = solarSystemID;
            return ret;
        }

        public decimal CalculatedCost(int systemSize, bool includeFee = false)
        {
            return CalculatedCost((long)systemSize, null, includeFee);
        }

        public decimal GetCost()
        {
            return CalculatedCost(SolarSystem.SystemSize);
        }

        public decimal CalculatedCost(long systemSize, decimal? dealerFee = null, bool includeFee = false)
        {
            decimal cost = 0;
            switch (RateType)
            {
                case AdderItemRateType.Flat:
                    cost = TotalCost;
                    break;
                case AdderItemRateType.PerKw:
                    cost = ((decimal)systemSize / 1000) * TotalCost;
                    break;
                case AdderItemRateType.PerWatt:
                    cost = systemSize * TotalCost;
                    break;
            }

            var feeValue = FinancingFee;

            if(dealerFee.HasValue && dealerFee > 0 && !FinancingFee.HasValue)
            {
                feeValue = dealerFee;
            }

            return cost + (includeFee ? ((feeValue ?? 0) / 100) * cost : 0);
        }

        public T GetCustomSettings<T>()
        {
            if (string.IsNullOrWhiteSpace(DynamicSettingsJSON))
            {
                return default(T);
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(DynamicSettingsJSON);
            }
            catch { }
            return default(T);
        }

        private List<AdderItemDynamicUserDataDataView> _dynamicSettings;
        public List<AdderItemDynamicUserDataDataView> GetDynamicSettings()
        {
            if (_dynamicSettings != null)
            {
                return _dynamicSettings;
            }
            _dynamicSettings = GetCustomSettings<List<AdderItemDynamicUserDataDataView>>();
            return _dynamicSettings;
        }

        private bool HasOption(string name, string value)
        {
            return GetDynamicSettings()?
                        .Any(ds => ds.Name == name && ds.SelectedOptions?.Contains(value, StringComparer.OrdinalIgnoreCase) == true) == true;
        }

        private bool HasBoolValue(string name, bool value)
        {
            return GetDynamicSettings()?
                        .Any(ds => ds.Name == name
                                && ds.Type == AdderItemDynamicItemType.Boolean
                                && ds.Value?.IsTrue() == value) == true;
        }

        public bool IsProposalIncentive(bool isRebate = false)
        {
            return (Type == AdderItemType.Incentive
                    && (IsRebate ?? false) == isRebate
                    // add exceptions that make this a non-proposal incentive below
                    && !HasBoolValue(DynamicIncentive_Names.TriSMART_Incentives_SummaryOnly, true));
        }

        public string ReductionDescription()
        {
            if (!ReducesUsage || !UsageReductionType.HasValue)
            {
                return null;
            }

            return UsageReductionType == AdderItemReducedAmountType.Flat ? $"Reduces consumption by {TotalReducedAmount} kWh" : $"Reduces consumption by {TotalReducedAmount} %";
        }

        public bool GetIsCalculatedPerRoofPlane()
        {
            if (CalculatedPer.HasValue)
            {
                return CalculatedPer == AdderItemCalculatedPerType.PerRoofPlane;
            }
            return IsCalculatedPerRoofPlane;
        }
    }
}
