using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.Sync.Contracts.Models
{
    [DataContract]
    public enum SessionStatus
    {
        [EnumMember]
        Started,

        [EnumMember]
        InProgress,

        [EnumMember]
        Ended,

        [EnumMember]
        Droped,

        [EnumMember]
        Error
    }

    [DataContract(IsReference = true)]
    public class SyncSession
    {
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index("CLUSTERED_INDEX_ON_LONG", IsClustered = true)]
        public long Id { get; set; }

        [DataMember]
        [Key, Required]
        public Guid Guid { get; set; }

        [DataMember]
        public Guid UserGuid { get; set; }

        [DataMember]
        public Guid DeviceGuid { get; set; }

        [DataMember]
        public DateTime DateCreated { get; set; }

        [DataMember]
        public SessionStatus Status { get; set; }

        [DataMember]
        public DateTime? DateEnd { get; set; }

        [DataMember]
        [NotMapped]
        public int DeltaCount { get; set; }

        #region Navigation

        [DataMember]
        [InverseProperty("SyncSession")]
        public List<Delta> Deltas { get; set; }

        #endregion
    }
}
