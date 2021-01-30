using System.ServiceModel;
using DataReef.TM.Models;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System;
using DataReef.Core.Enums;
using DataReef.TM.Models.PushNotifications;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IDeviceService : IDataService<Device>
    {
        [OperationContract]
        Task<bool> Validate();

        [OperationContract]
        void RegisterAPNDeviceToken(string deviceToken);

        [OperationContract]
        void UnRegisterAPNDeviceToken();

        /// <summary>
        /// Retrieves a list of Device Tokens for given UserID or for SmartPrincipal.UserID if userId is null
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A list of Apple Push Notification Device Tokens</returns>
        [OperationContract]
        List<string> GetAPNDeviceTokens(Guid? userId = null);

        [OperationContract]
        List<Tuple<string, NotificationType>> GetPushSubscriptions<T>(string entityId);

        [OperationContract]
        List<Tuple<string, NotificationType>> GetPushSubscriptionsForMultipleIds<T>(List<string> entityIds);

        [OperationContract]
        void PushToSubscribers<S, P>(string subscriptionId, string payloadId, DataAction action, string parentId = null, string alert = null);
    }
}