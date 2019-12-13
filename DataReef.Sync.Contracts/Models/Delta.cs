using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DataReef.Core.Enums;

namespace DataReef.Sync.Contracts.Models
{
    [DataContract]
    public class Delta
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [DataMember]
        [Key, Column(Order = 0)]
        public Guid UserGuid { get; set; }

        [DataMember]
        [Key, Column(Order = 1)]
        public Guid DeviceGuid { get; set; }

        [DataMember]
        [Key, Column(Order = 2)]
        public Guid EntityGuid { get; set; }

        [DataMember]
        public string EntityType { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public DataAction DataAction { get; set; }

        [DataMember]
        public Guid? SyncSessionGuid { get; set; }

        [DataMember]
        public DateTime DateCreated { get; set; }

        [DataMember]
        public long TenantId { get; set; }

        #region Navigation

        [ForeignKey("SyncSessionGuid")]
        public SyncSession SyncSession { get; set; }

        #endregion
    }
}
