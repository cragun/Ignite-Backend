using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models;
using System;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IAppSettingService : IDataService<AppSetting>
    {
        [OperationContract]
        string GetValue(string key);

        [OperationContract]
        void SetValue(string key, string value);

        [OperationContract]
        [AnonymousAccess]
        bool IsHealthy();

        [OperationContract]
        [AnonymousAccess]
        Version GetMinimumRequiredVersionForIPad();

        [OperationContract]
        [AnonymousAccess]
        bool TestPushNotifications(string token, string payload);
    }
}
