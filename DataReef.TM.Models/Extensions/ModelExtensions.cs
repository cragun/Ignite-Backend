using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.DTOs.OUs;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.DTOs.Signatures.Proposals;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ModelExtensions
{
    public static string GetByKey(this Dictionary<string, ValueTypePair<SettingValueType, string>> ouSettings, string key)
    {
        if (ouSettings == null || !ouSettings.ContainsKey(key))
            return null;

        return ouSettings.FirstOrDefault(s => s.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase)).Value.Value;
    }

    public static int? GetIntByKey(this Dictionary<string, ValueTypePair<SettingValueType, string>> ouSettings, string key, int? defaultValue = null)
    {
        var value = ouSettings.GetDoubleByKey(key);
        return value.HasValue ? (int)value : defaultValue;
    }

    public static double? GetDoubleByKey(this Dictionary<string, ValueTypePair<SettingValueType, string>> ouSettings, string key, double? defaultValue = null)
    {
        var value = ouSettings.GetByKey(key);
        if (value == null)
        {
            return defaultValue;
        }
        double ret = 0;

        if (double.TryParse(value, out ret))
        {
            return ret;
        }
        return defaultValue;
    }

    public static bool? GetBoolByKey(this Dictionary<string, ValueTypePair<SettingValueType, string>> ouSettings, string key)
    {
        var value = ouSettings.GetByKey(key);

        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }
        value = value.ToLower();
        return value == "1" || value == "true" || value == "yes";
    }

    public static T GetByKey<T>(this Dictionary<string, ValueTypePair<SettingValueType, string>> ouSettings, string key)
    {
        var value = ouSettings.GetByKey(key);
        if (value == null)
            return default(T);
        try
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
        catch { }
        return default(T);
    }

    public static string GetValueByName(this List<OrgSettingDataView> items, string key)
    {
        if (items == null)
        {
            return null;
        }

        return items
                   .FirstOrDefault(i => i.Name.Equals(key, StringComparison.OrdinalIgnoreCase))?
                   .Value;
    }

    public static double? GetValueAsDoubleByName(this List<OrgSettingDataView> items, string key)
    {
        var value = items.GetValueByName(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        double ret;
        if (double.TryParse(value, out ret))
        {
            return ret;
        }
        return null;
    }

    public static bool GetValueAsBool(this List<OUSetting> settings, string name, bool defaultValue = false)
    {
        var item = settings?.FirstOrDefault(s => s.Name == name);
        if (item == null)
        {
            return defaultValue;
        }

        return item.Value.IsTrue();
    }

    public static T GetValue<T>(this List<OUSetting> settings, string name)
    {
        var item = settings?.FirstOrDefault(s => s.Name == name);
        if (item == null)
        {
            return default(T);
        }
        try
        {
            return JsonConvert.DeserializeObject<T>(item.Value);
        }
        catch { }
        return default(T);
    }

    public static ProposalMediaItemDataViewType ToProposalEnum(this UserInputDataType value)
    {
        switch (value)
        {
            case UserInputDataType.None:
                return ProposalMediaItemDataViewType.None;
            case UserInputDataType.Signature:
                return ProposalMediaItemDataViewType.UserInput_Signature;
            case UserInputDataType.SalesRepSignature:
                return ProposalMediaItemDataViewType.SalesRepInput_Signature;
            case UserInputDataType.ThreeDCustomer:
                return ProposalMediaItemDataViewType.ThreeD_Customer_Signature;
            case UserInputDataType.ThreeDSalesRep:
                return ProposalMediaItemDataViewType.ThreeD_SalesRep_Signature;
            case UserInputDataType.EnergyBill:
                return ProposalMediaItemDataViewType.EnergyBill;
        }
        return ProposalMediaItemDataViewType.None;
    }

    public static Dictionary<Guid, int> OURoleWeights = new Dictionary<Guid, int>
    {
        { OURole.SuperUserID, 0 },
        { OURole.OwnerRoleID, 10 },
        { OURole.AdministratorRoleID, 20},
        { OURole.MemberRoleID, 30},
        { OURole.UserRoleID, 40},
    };

    public static bool IsGreaterThan(this Guid ouroleId, Guid comparisonRoleId)
    {
        var first = OURoleWeights.ContainsKey(ouroleId) ? OURoleWeights[ouroleId] : 100;
        var second = OURoleWeights.ContainsKey(comparisonRoleId) ? OURoleWeights[comparisonRoleId] : 100;

        return first >= second;
    }

    public static bool IsOwnerOrAdmin(this Guid ouRoleId)
    {
        return (OURoleWeights.ContainsKey(ouRoleId) ? OURoleWeights[ouRoleId] : 100) <= 20;
    }

    public static OUsAndRoleTree GetByOUID(this List<OUsAndRoleTree> ousTree, Guid ouid)
    {
        return ousTree?.SelectMany(o => o.GetOUAndChildren())?.FirstOrDefault(o => o.OUID == ouid);
    }

    public static List<OUsAndRoleTree> GetAll(this List<OUsAndRoleTree> ousTree)
    {
        return ousTree?.SelectMany(o => o.GetOUAndChildren())?.ToList();
    }

    /// <summary>
    /// Gets the usage reduction for this adder item
    /// </summary>
    /// <param name="adderItem"></param>
    /// <param name="usageInKwh"></param>
    /// <returns></returns>
    public static decimal GetAdderUsageReduction(this AdderItem adderItem, decimal usageInKwh)
    {
        if (adderItem.UsageReductionAmount == null || adderItem.UsageReductionType == null)
            return 0;

        var reducedAmount = adderItem.UsageReductionType.Value == AdderItemReducedAmountType.Flat
            ? adderItem.TotalReducedAmount
            : usageInKwh * adderItem.TotalReducedAmount / 100;

        return reducedAmount;
    }

    /// <summary>
    /// Gets the costs of the adder item
    /// </summary>
    /// <param name="adderItem"></param>
    /// <returns></returns>
    public static decimal GetAdderCosts(this AdderItem adderItem)
    {
        if (adderItem.Cost == 0 || adderItem.Quantity == 0)
            return 0;

        decimal systemAdderCost = 0;
        if (adderItem.RoofPlaneDetails == null || !adderItem.RoofPlaneDetails.Any())
        {
            systemAdderCost = adderItem.GetCost();
        }
        else
        {
            foreach (var roofPlaneDetail in adderItem.RoofPlaneDetails.Where(rp => rp.RoofPlane.SolarPanel != null))
            {
                decimal quantity = roofPlaneDetail.Quantity;
                switch (adderItem.RateType)
                {
                    case AdderItemRateType.Flat:
                        break;
                    case AdderItemRateType.PerKw:
                        quantity *= ((decimal)roofPlaneDetail.RoofPlane.SolarPanel.Watts / 1000) * roofPlaneDetail.RoofPlane.PanelsCount;
                        break;
                    case AdderItemRateType.PerWatt:
                        quantity *= (decimal)roofPlaneDetail.RoofPlane.SolarPanel.Watts * roofPlaneDetail.RoofPlane.PanelsCount;
                        break;
                }

                systemAdderCost += quantity * adderItem.Cost;
            }
        }

        return systemAdderCost;
    }
}
