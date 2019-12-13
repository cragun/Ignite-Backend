using System;
using System.ServiceModel;
using DataReef.Sync.Contracts.Models;
using System.Collections.Generic;
namespace DataReef.Sync.Contracts
{
    /// <summary>
    /// SyncService contract for performing the sync operation
    /// </summary>
    [ServiceContract]
    public interface ISyncDeltaService
    {
        /// <summary>
        /// Clear any pending deltas for a given user-device
        /// </summary>
        /// <param name="userGuid"></param>
        /// <param name="deviceGuid"></param>
        /// <returns>Number of entries deleted</returns>
        [OperationContract]
        int ClearDeltas(Guid userGuid, Guid deviceGuid);

        /// <summary>
        /// Begin sync
        /// </summary>
        /// <param name="userGuid"></param>
        /// <param name="deviceGuid"></param>
        /// <returns>Sync session GUID</returns>
        [OperationContract]
        SyncSession BeginSync(Guid userGuid, Guid deviceGuid);

        [OperationContract]
        SyncSession GetSyncSession(Guid userGuid, Guid deviceGuid, Guid sessionGuid);

        /// <summary>
        /// Update changes and compute deltas
        /// </summary>
        /// <param name="userGuid"></param>
        /// <param name="deviceGuid"></param>
        /// <param name="sessionGuid"></param>
        /// <param name="pageNumber"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns>Get all the updates for the current device</returns>
        [OperationContract]
        IEnumerable<Delta> GetDeltas(Guid userGuid, Guid deviceGuid, Guid sessionGuid, int pageNumber, int itemsPerPage);

        /// <summary>
        /// End sync session. Delete all synchronization informatio for this user
        /// </summary>
        /// <param name="userGuid"></param>
        /// <param name="deviceGuid"></param>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        [OperationContract]
        bool EndSync(Guid userGuid, Guid deviceGuid, Guid sessionGuid);
    }
}
