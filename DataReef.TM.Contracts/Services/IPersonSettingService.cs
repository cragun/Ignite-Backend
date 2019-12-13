using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPersonSettingService : IDataService<PersonSetting>
    {
        [OperationContract]
        Dictionary<string, ValueTypePair<SettingValueType, string>> GetSettings(Guid personID, PersonSettingGroupType? group);

        [OperationContract]
        void SetSettings(Guid personID, PersonSettingGroupType? group, Dictionary<string, ValueTypePair<SettingValueType, string>> values);
    }
}