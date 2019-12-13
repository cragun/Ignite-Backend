using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DataReef.TM.Models.Enums;

namespace DataReef.TM.Models
{
    [Table("EpcStatuses", Schema = "EPC")]
    public class EpcStatus : EntityBase
    {
        [DataMember]
        public Guid PropertyID { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public EpcStatusProgress StatusProgress { get; set; }

        [DataMember]
        public DateTime? CompletionDate { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey(nameof(PropertyID))]
        public Property Property { get; set; }

        [DataMember]
        [ForeignKey(nameof(PersonID))]
        public Person Person { get; set; }

        #endregion
    }
}
