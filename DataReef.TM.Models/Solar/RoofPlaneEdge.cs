using DataReef.TM.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("RoofPlaneEdges", Schema = "solar")]
    public class RoofPlaneEdge : EntityBase
    {
        [DataMember]
        public RoofPanelEdgeType EdgeType { get; set; }

        [DataMember]
        public double StartPointLatitude { get; set; }
        [DataMember]
        public double StartPointLongitude { get; set; }
        [DataMember]
        public double EndPointLatitude { get; set; }
        [DataMember]
        public double EndPointLongitude { get; set; }

        [DataMember]
        public double FireOffset { get; set; }
        [DataMember]
        public bool FireOffsetIsEnabled { get; set; }

        [DataMember]
        public int Index { get; set; }

        [DataMember]
        public Guid RoofPlaneID { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("RoofPlaneID")]
        public RoofPlane RoofPlane { get; set; }

        #endregion

        public RoofPlaneEdge Clone(Guid roofPlaneID)
        {
            RoofPlaneEdge ret = (RoofPlaneEdge)this.MemberwiseClone();
            ret.Reset();
            ret.RoofPlaneID = roofPlaneID;
            ret.RoofPlane = null;
            return ret;
        }
    }
}
