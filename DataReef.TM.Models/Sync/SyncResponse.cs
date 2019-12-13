using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DataReef.TM.Models.Enums;

namespace DataReef.TM.Models.Sync
{
    /// <summary>
    /// Server sync response base
    /// </summary>
    [DataContract]
    public class SyncResponse
    {
        /// <summary>
        /// identifies the Sync Session.  Sync sessions must be created before communicating with the api. sync sessions must be completed (delete) 
        /// when upon client receiving data
        /// </summary>
        [DataMember]
        public Guid SessionGuid { get; set; }

        /// <summary>
        /// Number of active deltas in the current session
        /// </summary>
        [DataMember]
        public int DeltaCount { get; set; }

        /// <summary>
        /// Status of the sync request
        /// </summary>
        [DataMember]
        public SyncStatus SyncStatus { get; set; }

        /// <summary>
        /// String message
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }

    /// <summary>
    /// Server PUT response
    /// </summary>
    [DataContract]
    public class SyncPutResponse : SyncResponse
    {
        /// <summary>
        /// Server returns a list of statuses for each sync
        /// </summary>
        [DataMember]
        public IEnumerable<SyncItemStatus> SyncStatuses { get; set; }
    }

    /// <summary>
    /// Server PUT response
    /// </summary>
    [DataContract]
    public class SyncGetResponse : SyncResponse
    {
        /// <summary>
        /// Server returns a list of statuses for each sync
        /// </summary>
        [DataMember]
        public IEnumerable<SyncItem> SyncItems { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class SyncItemStatus
    {
        [DataMember]
        public SyncItem SyncItem { get; set; }

        [DataMember]
        public ItemSyncStatus ItemSyncStatus { get; set; }

    }
}
