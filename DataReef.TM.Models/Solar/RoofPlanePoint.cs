using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("RoofPlanePoints", Schema = "solar")]
    public class RoofPlanePoint : EntityBase
    {
        [DataMember]
        public int Index { get; set; }

        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }

        [DataMember]
        public Guid RoofPlaneID { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("RoofPlaneID")]
        public RoofPlane RoofPlane { get; set; }

        #endregion

        public RoofPlanePoint Clone(Guid roofPlaneID)
        {
            RoofPlanePoint ret = (RoofPlanePoint)this.MemberwiseClone();
            ret.Reset();
            ret.RoofPlaneID = roofPlaneID;
            ret.RoofPlane = null;
            return ret;
        }

    }
}
