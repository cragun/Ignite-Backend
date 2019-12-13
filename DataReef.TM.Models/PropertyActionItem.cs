using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DataReef.TM.Models.Enums;

namespace DataReef.TM.Models
{
    [Table("ActionItems", Schema = "EPC")]
    public class PropertyActionItem : EntityBase
    {
        [DataMember]
        public Guid PropertyID { get; set; }

        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public ActionItemStatus Status { get; set; }

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
