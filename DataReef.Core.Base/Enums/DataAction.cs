using System.Runtime.Serialization;

namespace DataReef.Core.Enums
{
    /// <summary>
    /// CRUD operation enum
    /// </summary>
    [DataContract]
    public enum DataAction
    {
        /// <summary>
        /// Default
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// Insert
        /// </summary>
        [EnumMember]
        Insert = 1,

        /// <summary>
        /// Update
        /// </summary>
        [EnumMember]
        Update = 2,

        /// <summary>
        /// Delete
        /// </summary>
        [EnumMember]
        Delete = 3
    }
}
