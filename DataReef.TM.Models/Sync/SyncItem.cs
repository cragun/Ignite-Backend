using System;
using System.Runtime.Serialization;
using DataReef.Core.Enums;

namespace DataReef.TM.Models.Sync
{
    /// <summary>
    /// Simple abstraction to represent a sync item.
    /// </summary>
    [DataContract]
    public class SyncItem
    {
        /// <summary>
        /// Entity unique guid
        /// </summary>
        [DataMember]
        public Guid EntityGuid { get; set; }

        /// <summary>
        /// Entity type
        /// </summary>
        [DataMember]
        public string EntityType { get; set; }

        /// <summary>
        /// Action that was performed on this item
        /// </summary>
        [DataMember]
        public DataAction DataAction { get; set; }

        /// <summary>
        /// Item version
        /// </summary>
        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public EntityBase Entity { get; set; }
    }
}
