using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("RoofPlanePanels", Schema = "solar")]
    public class RoofPlanePanel : EntityBase
    {
        [DataMember]
        public double CenterX { get; set; }
        [DataMember]
        public double CenterY { get; set; }

        [DataMember]
        public double Height { get; set; }
        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public Guid RoofPlaneID { get; set; }

        [DataMember]
        public double TopLeftLat { get; set; }

        [DataMember]
        public double TopLeftLon { get; set; }

        [DataMember]
        public double BottomRightLat { get; set; }

        [DataMember]
        public double BottomRightLon { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("RoofPlaneID")]
        public RoofPlane RoofPlane { get; set; }

        #endregion

        public RoofPlanePanel Clone(Guid roofPlaneID)
        {
            RoofPlanePanel ret = (RoofPlanePanel)this.MemberwiseClone();
            ret.Reset();
            ret.RoofPlaneID = roofPlaneID;
            ret.RoofPlane = null;
            return ret;

        }

    }
}
