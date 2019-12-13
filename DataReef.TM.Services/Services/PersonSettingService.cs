using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Classes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.TM.Models.Enums;
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Repository;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PersonSettingService : DataService<PersonSetting>, IPersonSettingService
    {
        public PersonSettingService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
        }

        public override PersonSetting Insert(PersonSetting entity)
        {
            using (var dc = new DataContext())
            {
                var setting = dc
                                .PersonSettings
                                .FirstOrDefault(s => s.PersonID == entity.PersonID
                                                  && s.Name == entity.Name);
                if (setting != null)
                {
                    setting.CopyFrom(entity);
                    setting.DateLastModified = DateTime.UtcNow;
                    setting.Version += 1;
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

        public Dictionary<string, ValueTypePair<SettingValueType, string>> GetSettings(Guid personID, PersonSettingGroupType? group)
        {
            var data = GetPersonSettings(personID, group);
            return data.ToDictionary(d => d.Name, d => new ValueTypePair<SettingValueType, string>(d.ValueType, d.Value));
        }


        public void SetSettings(Guid personID, PersonSettingGroupType? group, Dictionary<string, ValueTypePair<SettingValueType, string>> values)
        {
            using (var dc = new DataContext())
            {
                var keys = values
                            .Keys
                            .ToList();

                var settings = dc
                                .PersonSettings
                                .Where(ps => ps.PersonID == personID
                                          && keys.Contains(ps.Name))
                                .ToList();

                foreach (var item in settings)
                {
                    var newSetting = values[item.Name];

                    item.Value = newSetting.Value;
                    item.ValueType = newSetting.Type;
                    item.Group = group ?? item.Group;
                    item.Updated(SmartPrincipal.UserId);
                }

                var existingSettings = settings
                                        .Select(s => s.Name)
                                        .ToList();

                var newSettings = keys
                                    .Where(k => !existingSettings.Contains(k))
                                    .ToList();

                foreach (var item in newSettings)
                {
                    var newSetting = values[item];

                    var setting = new PersonSetting
                    {
                        PersonID = personID,
                        Name = item,
                        Value = newSetting.Value,
                        ValueType = newSetting.Type,
                        CreatedByID = SmartPrincipal.UserId,
                        Group = group ?? PersonSettingGroupType.Prescreen
                    };
                    dc
                        .PersonSettings
                        .Add(setting);
                }

                dc.SaveChanges();
            }
        }

        public static List<PersonSetting> GetPersonSettings(Guid personID, PersonSettingGroupType? group = null, DataContext dc = null)
        {
            var needToDispose = dc == null;

            dc = dc ?? new DataContext();

            var settings = dc
                            .PersonSettings
                            .Where(ps => ps.PersonID == personID
                                    && (!group.HasValue
                                       || (group.HasValue && ps.Group == group.Value)))
                            .ToList();

            if (needToDispose)
            {
                dc.Dispose();
            }

            return settings;
        }
    }
}
