using DataReef.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [Table("Audits")]
    public class CRUDAudit : EntityBase
    {
        [DataMember, StringLength(100)]
        public string EntityName { get; set; }

        [DataMember]
        public Guid EntityID { get; set; }

        [DataMember]
        public string OldValue { get; set; }

        [DataMember]
        public string NewValue { get; set; }

        [DataMember]
        public CrudAction Action { get; set; }
    }
}
