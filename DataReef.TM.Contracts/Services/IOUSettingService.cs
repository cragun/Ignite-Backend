using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IOUSettingService : IDataService<OUSetting>
    {
        [OperationContract]
        List<OUSetting> GetSettingsByOUID(Guid ouId, OUSettingGroupType? group = null);

        [OperationContract]
        List<KeyValuePair<Guid, List<OUSetting>>> GetOuSettingsMany(IEnumerable<Guid> ouIds, OUSettingGroupType? group = null, bool includeDeleted = false);

        [OperationContract]
        List<OUSetting> GetSettingsByPropertyID(Guid propertyID, OUSettingGroupType? group = null);

        [OperationContract]
        Dictionary<string, ValueTypePair<SettingValueType, string>> GetSettings(Guid ouid, OUSettingGroupType? group);

        [OperationContract]
        void SetSettings(Guid ouid, OUSettingGroupType? group, Dictionary<string, ValueTypePair<SettingValueType, string>> values);

        [OperationContract]
        void UpdateEquipment();

        [OperationContract]
        void UpdateSBSettings();

        object GetOUSettingForPropertyID(Guid propertyID, string settingName, Type type);
        T GetOUSettingForPropertyID<T>(Guid propertyID, string settingName) where T : class;
    }
}