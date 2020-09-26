using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class Incentive
    {

        public Guid Guid { get; set; }
        public bool IsRebate { get; set; }
        public bool IsAppliedBeforeITC { get; set; }
        public AdderItemRateType RateType { get; set; }
        public AdderItemRecurrenceType RecurrenceType { get; set; }
        public int RecurrenceStart { get; set; }
        public int RecurrencePeriod { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public string DynamicSettingsJSON { get; set; }
        public string Name { get; set; }

        private int StartMonth => RecurrenceType == AdderItemRecurrenceType.OneTime
            ? 0
            : RecurrenceType == AdderItemRecurrenceType.Monthly
                ? RecurrenceStart
                : ((RecurrenceStart - 1) * 12) + 1;

        private int EndMonth => RecurrenceType == AdderItemRecurrenceType.OneTime
            ? 0
            : RecurrenceType == AdderItemRecurrenceType.Monthly
            ? RecurrenceStart + RecurrencePeriod
            : ((RecurrenceStart + RecurrencePeriod - 1) * 12) + 1;

        public decimal GetIncentiveValue(int monthIndex, long systemSize)
        {
            if (!IncentiveShouldApply(monthIndex))
            {
                return 0;
            }

            return GetTotal(systemSize);
        }

        public decimal GetTotal(long systemSize)
        {
            switch (RateType)
            {
                case AdderItemRateType.Flat:
                    return Cost * Quantity;
                case AdderItemRateType.PerKw:
                    return Cost * Quantity * ((decimal)systemSize / 1000);
                case AdderItemRateType.PerWatt:
                    return Cost * Quantity * systemSize;
            }
            return 0;
        }

        public bool IncentiveShouldApply(int monthIndex)
        {
            return monthIndex >= StartMonth && monthIndex < EndMonth;
        }

        /// <summary>
        /// Get the total incentives for all period
        /// </summary>
        /// <returns></returns>
        public decimal GetGrandTotal(long systemSize)
        {
            if (RecurrenceType == AdderItemRecurrenceType.OneTime)
            {
                return GetTotal(systemSize);
            }

            decimal result = 0;

            for (int i = StartMonth; i < EndMonth; i++)
            {
                result += GetIncentiveValue(i, systemSize);
            }

            return result;
        }

        private List<AdderItemDynamicUserDataDataView> _dynamicSettings;
        public List<AdderItemDynamicUserDataDataView> GetDynamicSettings()
        {
            if (_dynamicSettings != null)
            {
                return _dynamicSettings;
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(DynamicSettingsJSON))
                {
                    _dynamicSettings = JsonConvert.DeserializeObject<List<AdderItemDynamicUserDataDataView>>(DynamicSettingsJSON);
                }
            }
            catch { }
            return _dynamicSettings;
        }

        public List<AdderItemDynamicUserDataDataView> GetDynamicSettingsWithBoolValue(string name, bool value)
        {
            var dynSettings = GetDynamicSettings();

            return dynSettings?
                    .Any(ds => ds.Type == AdderItemDynamicItemType.Boolean
                            && ds.Name == name
                            && ds.Value.IsTrue() == value) == true
                    ? dynSettings
                    : null;
        }

    }
}
