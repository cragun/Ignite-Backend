using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    /// <summary>
    /// Synchronization request servcer status
    /// </summary>
    [DataContract]
    public enum SyncStatus
    {
        /// <summary>
        /// Ok, existing deltas are included in the package
        /// </summary>
        [EnumMember]
        Ok,

        /// <summary>
        /// Delta threshold has been reached. Force client for full sync
        /// </summary>
        [EnumMember]
        ForceFullSync,

        /// <summary>
        /// Client has sent an invalid session guid. Begin is required
        /// </summary>
        [EnumMember]
        InvalidSession,

        /// <summary>
        /// Unspecified error occured. 
        /// </summary>
        [EnumMember]
        Error
    }
}
