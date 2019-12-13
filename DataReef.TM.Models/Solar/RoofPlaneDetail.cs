using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("RoofPlaneDetails", Schema = "solar")]
    public class RoofPlaneDetail : EntityBase
    {
        [DataMember]
        public Guid AdderItemID { get; set; }

        [DataMember]
        public Guid RoofPlaneID { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey(nameof(AdderItemID))]
        public AdderItem AdderItem { get; set; }

        [DataMember]
        [ForeignKey(nameof(RoofPlaneID))]
        public RoofPlane RoofPlane { get; set; }

        #endregion
    }
}