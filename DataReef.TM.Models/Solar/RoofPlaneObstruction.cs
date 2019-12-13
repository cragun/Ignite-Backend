using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DataReef.Core.Attributes;
using DataReef.TM.Models.Enums;

namespace DataReef.TM.Models.Solar
{
    [Table("RoofPlaneObstructions", Schema = "solar")]
    public class RoofPlaneObstruction : EntityBase
    {
        [DataMember]
        public Guid RoofPlaneID { get; set; }

        [DataMember]
        public ObstructionType ObstructionType { get; set; }

        [DataMember]
        public float? Radius { get; set; }

        [DataMember]
        public double? CenterPointX { get; set; }

        [DataMember]
        public double? CenterPointY { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey(nameof(RoofPlaneID))]
        public RoofPlane RoofPlane { get; set; }

        [DataMember]
        [InverseProperty(nameof(ObstructionPoint.RoofPlaneObstruction))]
        [AttachOnUpdate]
        public ICollection<ObstructionPoint> ObstructionPoints { get; set; }

        #endregion

        public RoofPlaneObstruction Clone(Guid roofPlaneID)
        {

            if (this.ObstructionPoints == null) throw new MissingMemberException("Missing Obstruction Points In The Object Graph");

            RoofPlaneObstruction ret = (RoofPlaneObstruction)this.MemberwiseClone();
            ret.Reset();
            ret.RoofPlaneID = roofPlaneID;
            ret.RoofPlane = null;

            ret.ObstructionPoints = new List<ObstructionPoint>();
            foreach (var point in this.ObstructionPoints)
            {
                ret.ObstructionPoints.Add(point.Clone(this.Guid));
            }

            return ret;


        }
    }
}
