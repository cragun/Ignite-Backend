using System;
using System.Collections.Generic;
using DataReef.TM.Models;
using DataReef.TM.Models.Sync;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ISyncDataService
    {
        /// <summary>
        /// Build an initial synchronization package
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        InitDataPacket GetAll();

        [OperationContract]
        IEnumerable<SyncItemStatus> Update(IEnumerable<SyncItem> clientSyncPacket);

        [OperationContract]
        IEnumerable<SyncItem> GetServerSyncPacket(IEnumerable<SyncItem> entityDeltas);

        /// <summary>
        /// Get the list of affected userdevices for a specific entity
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<UserDevice> GetSyncDevices(Guid ouGuid, Guid entityGuid, string entityType);
    }
}