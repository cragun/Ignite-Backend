using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    /// <summary>
    /// Sync operation status enum
    /// </summary>
    [DataContract]
    public enum ItemSyncStatus
    {
        /// <summary>
        /// Sync operation success
        /// </summary>
        [EnumMember]
        Ok,

        /// <summary>
        /// Sync operation resulted in an error
        /// </summary>
        [EnumMember]
        Error,

        /// <summary>
        /// Sync operation cannot be performed due to version conflict
        /// </summary>
        [EnumMember]
        Conflict
    }

}
