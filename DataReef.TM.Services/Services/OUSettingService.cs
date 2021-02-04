
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.DTOs.FinanceAdapters.SST;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.DTOs.OUs;
using DataReef.TM.Models.Enums;
using DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class OUSettingService : DataService<OUSetting>, IOUSettingService
    {

        private readonly Lazy<ICRUDAuditService> _auditService;
        public OUSettingService(ILogger logger,
                                Func<IUnitOfWork> unitOfWorkFactory,
                                Lazy<ICRUDAuditService> auditService)
            : base(logger, unitOfWorkFactory)
        {
            _auditService = auditService;
        }

        public override OUSetting Insert(OUSetting entity)
        {
            using (var dc = new DataContext())
            {
                if (entity.Name.Equals("Contractor ID", StringComparison.InvariantCulture))
                {
                    if (dc.OUSettings.Any(s => s.Name == "Contractor ID" && s.Value == entity.Value))
                        throw new Exception($"Contractor ID {entity.Value} already defined");
                }
                //if (entity.Name.Equals(OUSetting.Financing_Options, StringComparison.OrdinalIgnoreCase))
                //{
                //    UpdateChildOUFinancingOptionSettings(entity);
                //}

                var setting = dc
                            .OUSettings
                            .FirstOrDefault(s => s.OUID == entity.OUID
                                              && s.Name == entity.Name);
                if (setting != null)
                {
                    setting.CopyFrom(entity);
                    setting.IsDeleted = false;
                    setting.Updated(SmartPrincipal.UserId);
                    dc.SaveChanges();
                    setting.SaveResult = SaveResult.SuccessfulUpdate;
                    return setting;
                }
                else
                {
                    return base.Insert(entity);
                }
            }
        }

        public override OUSetting Update(OUSetting entity)
        {
            using (var ctx = new DataContext())
            {
                var oldSetting = ctx.OUSettings.FirstOrDefault(os => os.Guid == entity.Guid);
                _auditService.Value.UpdateValue(oldSetting.Guid, oldSetting.Name, nameof(OUSetting), oldSetting.Value, entity.Value, "update ousetting API");
            }

            using (var context = new DataContext())
            {
                if (entity.Name.Equals("Contractor ID", StringComparison.InvariantCulture))
                {
                    if (context.OUSettings.Any(s => s.Name == "Contractor ID" && s.Value == entity.Value))
                        throw new Exception($"Contractor ID {entity.Value} already defined");
                }

                //if (entity.Name.Equals(OUSetting.Financing_Options, StringComparison.OrdinalIgnoreCase))
                //{
                //    UpdateChildOUFinancingOptionSettings(entity);
                //}

                return base.Update(entity, context);
            }
        }

        public override ICollection<SaveResult> DeleteMany(Guid[] uniqueIds)
        {
            // For now we'll ignore deletion for OUSettings.
            return uniqueIds?.Select(id => SaveResult.SuccessfulDeletion)?.ToList();
        }

        protected override void OnDeletedMany(OUSetting[] entities)
        {
            _auditService.Value.DeleteEntities(entities, "Delete OUSetting API");
        }

        public static void UpdateChildOUFinancingOptionSettings(OUSetting entity, DataContext context = null)
        {
            var needToDispose = context == null;

            context = context ?? new DataContext();

            var originalEntity = context
                                            .OUSettings
                                            .FirstOrDefault(ous => ous.Guid == entity.Guid);

            var originalSett = originalEntity?.GetValue<List<FinancingSettingsDataView>>();

            var newSett = entity.GetValue<List<FinancingSettingsDataView>>();
            List<Guid> newPlanIDs = null;
            List<Guid> removedPlanIDs = null;

            if (originalSett == null)
            {
                newPlanIDs = newSett?
                                .Select(s => s.PlanID)
                                .ToList();
                removedPlanIDs = new List<Guid>();
            }

            if (originalSett != null && newSett != null)
            {
                newPlanIDs = newSett
                                .Where(s => s.GetIsEnabled()
                                         && !originalSett.Select(os => os.PlanID).Contains(s.PlanID))
                                .Select(s => s.PlanID)
                                .ToList();

                var newlyEnabledPlanIDs = newSett
                                .Where(s => s.GetIsEnabled()
                                         && originalSett
                                                .Where(os => !os.GetIsEnabled())
                                                .Select(os => os.PlanID)
                                                .Contains(s.PlanID))
                                .Select(s => s.PlanID)
                                .ToList();

                removedPlanIDs = originalSett
                                .Where(os => !newSett.Select(ns => ns.PlanID).Contains(os.PlanID))
                                .Select(os => os.PlanID)
                                .ToList();

                var newlyDisabledPlanIDs = newSett
                                .Where(ns => !ns.GetIsEnabled()
                                          && originalSett
                                                .Where(os => os.GetIsEnabled())
                                                .Select(os => os.PlanID)
                                                .Contains(ns.PlanID))
                                .Select(s => s.PlanID)
                                .ToList();

                newPlanIDs = newPlanIDs
                                .Union(newlyEnabledPlanIDs)
                                .ToList();

                removedPlanIDs = removedPlanIDs
                                .Union(newlyDisabledPlanIDs)
                                .ToList();

            }

            // get all ancestor OUIDs and exclude ouid
            var childIDs = context
                        .Database
                        .SqlQuery<Guid>($"select * from OUTree('{entity.OUID}')")
                        .Except(new List<Guid> { entity.OUID })
                        .ToList();

            var financingSettings = context
                        .OUSettings
                        .Where(ous => childIDs.Contains(ous.OUID) && ous.Name == OUSetting.Financing_Options)
                        .ToList();

            foreach (var fin in financingSettings)
            {
                var hasChanged = false;
                var childFinOptions = fin.GetValue<List<FinancingSettingsDataView>>() ?? new List<FinancingSettingsDataView>();
                var childPlanIds = childFinOptions
                                    .Select(c => c.PlanID)
                                    .ToList();
                var count = childFinOptions?.Max(c => c.GetOrder()) + 1 ?? 1;

                var toAdd = newPlanIDs
                                .Where(pID => !childPlanIds.Contains(pID))
                                .Select((planId, idx) => new FinancingSettingsDataView(planId, true, count + idx, newSett.FirstOrDefault(ns => ns.PlanID == planId)?.Data))
                                .ToList();

                if (toAdd?.Count > 0)
                {
                    childFinOptions.AddRange(toAdd);
                    count += toAdd.Count + 1;
                    hasChanged = true;
                }

                var toRemove = childFinOptions
                                .Where(cfo => removedPlanIDs.Contains(cfo.PlanID))
                                .ToList();
                foreach (var item in toRemove)
                {
                    childFinOptions.Remove(item);
                    childFinOptions
                            .Where(cfo => cfo.GetOrder() > item.GetOrder())
                            .ToList()
                            .ForEach(cfo => cfo.SetOrder(cfo.GetOrder() - 1));
                    hasChanged = true;
                }

                if (hasChanged)
                {
                    fin.Value = JsonConvert.SerializeObject(childFinOptions);
                }
            }


            if (needToDispose)
            {
                context.SaveChanges();
                context.Dispose();
            }
        }

        public List<OUSetting> GetSettingsByOUID(Guid ouId, OUSettingGroupType? group = null)
        {
            return Task.Run(() => GetOuSettings(ouId, group)).Result;
        }

        public void UpdateSBSettings()
        {
            using (var uow = UnitOfWorkFactory())
            {
                var settings = uow
                            .Get<OUSetting>()
                            .Where(ous => ous.Name.StartsWith("Integrations."))
                            .ToList();

                var groups = settings.GroupBy(s => s.OUID);

                foreach (var group in groups)
                {
                    if (group.Any(g => g.Name == "Integrations.Options.Selected"))
                    {
                        continue;
                    }

                    var sbSetting = group.FirstOrDefault(g => g.Name == "Integrations.SMARTBoard.Settings");
                    if (sbSetting == null)
                    {
                        continue;
                    }

                    var settingValue = sbSetting.GetValue<SSTSettings>();
                    var apiKey = settingValue?.ApiKey;
                    if (string.IsNullOrEmpty(apiKey))
                    {
                        continue;
                    }

                    var valueObj = new SelectedIntegrationOption
                    {
                        Id = "smartBOARD-integration",
                        Name = "SmartBOARD",
                        Data = new IntegrationOptionData
                        {
                            SMARTBoard = new SMARTBoardIntegrationOptionData
                            {
                                BaseUrl = SolarTrackerResources.BaseUrl,
                                ApiKey = apiKey,
                                HomeUrl = "/leads/view/{0}",
                                CreditApplicationUrl = "SB_CreditApplicationUrl"
                            }
                        }
                    };

                    var newSetting = new OUSetting
                    {
                        OUID = sbSetting.OUID,
                        Value = JsonConvert.SerializeObject(valueObj),
                        CreatedByID = sbSetting.CreatedByID,
                        Name = "Integrations.Options.Selected",
                        Group = OUSettingGroupType.DealerSettings,
                        Inheritable = true,
                        ValueType = SettingValueType.JSON,
                    };
                    uow.Add(newSetting);
                }

                uow.SaveChanges();
            }
        }

        public List<OUSetting> GetSettingsByPropertyID(Guid propertyID, OUSettingGroupType? group = null)
        {
            using (var dc = new DataContext())
            {
                var prop = dc
                    .Properties
                    .Include(p => p.Territory)
                    .FirstOrDefault(p => p.Guid == propertyID);
                if (prop?.Territory?.OUID == null)
                {
                    return null;
                }

                return Task.Run(() => GetOuSettings(prop.Territory.OUID, group, dc)).Result;
            }
        }

        public object GetOUSettingForPropertyID(Guid propertyID, string settingName, Type type)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var ouid = uow
                    .Get<Property>()
                    .Include(p => p.Territory)
                    .FirstOrDefault(p => p.Guid == propertyID)?
                    .Territory?
                    .OUID;
                if (!ouid.HasValue)
                {
                    return null;
                }

                var settings = Task.Run(() => GetOuSettings(ouid.Value)).Result;
                return settings?
                            .FirstOrDefault(s => s.Name == settingName)?
                            .GetValue(type);
            }
        }

        public T GetOUSettingForPropertyID<T>(Guid propertyID, string settingName) where T : class
        {
            using (var uow = UnitOfWorkFactory())
            {
                var ouid = uow
                    .Get<Property>()
                    .Include(p => p.Territory)
                    .FirstOrDefault(p => p.Guid == propertyID)?
                    .Territory?
                    .OUID;
                if (!ouid.HasValue)
                {
                    return default(T);
                }

                var settings = Task.Run(() => GetOuSettings(ouid.Value)).Result;
                return settings?
                            .FirstOrDefault(s => s.Name == settingName)?
                            .GetValue<T>();
            }
        }

        public static async Task<List<OUSetting>> GetOuSettings(Guid ouId, OUSettingGroupType? group = null, DataContext dc = null, bool includeDeleted = false)
        {
            var result = new List<OUSetting>();

            var needToDispose = dc == null;

            dc = dc ?? new DataContext();

            var settings = await dc
                        .Database
                        .SqlQuery<OUSetting>("exec proc_OUSettings @ouid, @IncludeDeleted", new SqlParameter("@ouid", ouId), new SqlParameter("@IncludeDeleted", includeDeleted))
                        .ToListAsync();

            if (group.HasValue)
            {
                settings = settings
                            .Where(s => s.Group == group)
                            .ToList();
            }

            foreach (var item in settings)
            {
                var settingExists = result.FirstOrDefault(r => r.Name == item.Name);

                // if setting already exists 
                if (settingExists != null)
                {
                    result.Remove(settingExists);
                }
                // only add the setting if it's ment for this OU, or if it's inheritable
                if (item.OUID == ouId || (item.OUID != ouId && item.Inheritable))
                {
                    result.Add(item);
                }
            }

            if (needToDispose)
            {
                dc.Dispose();
            }

            return result;
        }

        public List<KeyValuePair<Guid, List<OUSetting>>> GetOuSettingsMany(IEnumerable<Guid> ouIds, OUSettingGroupType? group = null, bool includeDeleted = false)
        {
            var result = new List<KeyValuePair<Guid, List<OUSetting>>>();

            var dc = new DataContext();

            var requestDataTable = new DataTable();
            requestDataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
            foreach (var id in ouIds)
            {
                var row = requestDataTable.NewRow();
                row["Id"] = id;
                requestDataTable.Rows.Add(row);
            }

            var settings = dc
                        .Database
                        .SqlQuery<OrgIdSetting>("exec proc_OUSettingsMany @OUIDs, @IncludeDeleted",
                            new SqlParameter
                            {
                                ParameterName = "@OUIDs",
                                Value = requestDataTable,
                                SqlDbType = SqlDbType.Structured,
                                TypeName = "dbo.GuidList"
                            },
                            new SqlParameter("@IncludeDeleted", includeDeleted))
                        .OrderByDescending(s => s.Level)
                        .ToList();

            foreach (var orgSettings in settings?.GroupBy(x => x.OrgId))
            {
                var settingsItems = orgSettings.ToList();
                if (group.HasValue)
                {
                    settingsItems = orgSettings
                                .Where(s => s.Group == group)
                                .ToList();
                }

                var settingsKeyValue = new KeyValuePair<Guid, List<OUSetting>>(orgSettings.Key, new List<OUSetting>());

                foreach (var item in settingsItems)
                {
                    var settingExists = settingsKeyValue.Value.FirstOrDefault(r => r.Name == item.Name);

                    // if setting already exists 
                    if (settingExists != null)
                    {
                        settingsKeyValue.Value.Remove(settingExists);
                    }
                    // only add the setting if it's ment for this OU, or if it's inheritable
                    if (item.OUID == orgSettings.Key || (item.OUID != orgSettings.Key && item.Inheritable))
                    {
                        settingsKeyValue.Value.Add(item);
                    }
                }

                result.Add(settingsKeyValue);
            }

            dc.Dispose();

            return result;
        }

        public async Task<Dictionary<string, ValueTypePair<SettingValueType, string>>> GetSettings(Guid ouid, OUSettingGroupType? group)
        {
            var data = await GetOuSettings(ouid, group);
            return data.ToDictionary(d => d.Name, d => new ValueTypePair<SettingValueType, string>(d.ValueType, d.Value));
        }

        public void SetSettings(Guid ouid, OUSettingGroupType? group, Dictionary<string, ValueTypePair<SettingValueType, string>> values)
        {
            using (var dc = new DataContext())
            {
                var contractorValues = values.Where(v => v.Key == "Contractor ID").Select(v => v.Value.Value).ToList();
                if (dc.OUSettings.Any(s => s.Name == "Contractor ID" && contractorValues.Contains(s.Value)))
                    throw new Exception("Contractor ID already defined");

                var keys = values
                            .Keys
                            .ToList();

                // get current settings that match the keys
                var settings = dc
                                .OUSettings
                                .Where(s => s.OUID == ouid
                                         && keys.Contains(s.Name))
                                .ToList();
                // update current settings, if needed
                foreach (var item in settings)
                {
                    var value = values[item.Name];

                    _auditService.Value.UpdateValue(item.Guid, item.Name, nameof(OUSetting), item.Value, value.Value, "set OUSettings");

                    item.Value = value.Value;
                    item.ValueType = value.Type;
                    item.Group = group ?? item.Group;
                    item.IsDeleted = false;
                    item.Updated(SmartPrincipal.UserId);
                }
                var existingSettings = settings
                                        .Select(s => s.Name)
                                        .ToList();
                // get new keys, and create settings for them
                var newSettings = keys
                                    .Where(k => !existingSettings.Contains(k))
                                    .ToList();

                foreach (var item in newSettings)
                {
                    var val = values[item];
                    var sett = new OUSetting
                    {
                        OUID = ouid,
                        Name = item,
                        Value = val.Value,
                        ValueType = val.Type,
                        Inheritable = true,
                        CreatedByID = SmartPrincipal.UserId,
                        Group = group ?? OUSettingGroupType.ConfigurationFile
                    };

                    dc
                        .OUSettings
                        .Add(sett);
                }
                dc.SaveChanges();
            }
        }

        public void UpdateEquipment()
        {
            using (var uow = UnitOfWorkFactory())
            {
                // update solar panels
                UpdateEquipment(uow,
                        OUSetting.Solar_Panels,
                        OUSetting.NewSolar_Panels,
                        "IsEnabled",
                        (guid, index) => new SolarPanelsSettingDataView(guid, index),
                        (sett) =>
                        {
                            sett.Value = sett.Value.Replace("undefined", "0").Replace("null", "0");
                            var value = sett.GetValue<List<SolarPanelsSettingDataView>>();
                            value.ForEach(v => v.IsEnabled = "1");
                            sett.Value = JsonConvert.SerializeObject(value);
                        });

                // update inverters
                UpdateEquipment(uow,
                        OUSetting.Solar_Inverters,
                        OUSetting.NewSolar_Inverters,
                        "IsEnabled",
                        (guid, index) => new EnableableSettingDataView(guid, index),
                        (sett) =>
                        {
                            sett.Value = sett.Value.Replace("undefined", "0").Replace("null", "0");
                            var value = sett.GetValue<List<EnableableSettingDataView>>();
                            value.ForEach(v => v.IsEnabled = "1");
                            sett.Value = JsonConvert.SerializeObject(value);
                        });


                uow.SaveChanges();
            }
        }

        private void UpdateEquipment<T>(IUnitOfWork uow, string oldSettingName, string newSettingName, string missingKeyName, Func<Guid, int, T> getNewValue, Action<OUSetting> updateSetting)
        {
            var newSettingOUIDs = uow
                                        .Get<OUSetting>()
                                        .Where(ous => ous.Name == newSettingName)
                                        .Select(ous => ous.OUID)
                                        .Distinct()
                                        .ToList();

            var oldSettings = uow
                                    .Get<OUSetting>()
                                    .Where(ous => ous.Name == oldSettingName
                                                  && !newSettingOUIDs.Contains(ous.OUID))
                                    .ToList();

            var newSettings = new List<OUSetting>();

            foreach (var sett in oldSettings)
            {
                try
                {
                    var value = sett.GetValue<List<Guid>>();

                    var newValue = value.Select((v, idx) => getNewValue(v, idx)).ToList();

                    var newSett = new OUSetting
                    {
                        OUID = sett.OUID,
                        Name = newSettingName
                    };
                    newSett.CopyFrom(sett);
                    newSett.Value = JsonConvert.SerializeObject(newValue);
                    newSettings.Add(newSett);
                }
                catch { }
            }

            uow.AddMany(newSettings);

            var newIncompleteSettings = uow
                                    .Get<OUSetting>()
                                    .Where(ous => ous.Name == newSettingName && !ous.Value.Contains(missingKeyName))
                                    .ToList();

            foreach (var sett in newIncompleteSettings)
            {
                try
                {
                    updateSetting(sett);
                }
                catch { }
            }
        }
    }
}
