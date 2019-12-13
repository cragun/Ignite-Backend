using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Sync
{
    /// <summary>
    /// Client sync message request payload
    /// </summary>
    [DataContract]
    public class SyncPutRequest
    {
        [DataMember]
        public IEnumerable<SyncItem> SyncItems { get; set; }
    }
}
