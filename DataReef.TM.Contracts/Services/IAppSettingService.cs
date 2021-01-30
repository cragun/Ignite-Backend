﻿using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IAppSettingService : IDataService<AppSetting>
    {
        [OperationContract]
        Task<string> GetValue(string key);

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
        int GetLoginDays();

        [OperationContract]
        [AnonymousAccess]
        bool TestPushNotifications(string token, string payload);
    }
}
