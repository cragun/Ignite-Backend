using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("ObstructionPoints", Schema = "solar")]
    public class ObstructionPoint : EntityBase
    {
        [DataMember]
        public Guid RoofPlaneObstructionID { get; set; }

        [DataMember]
        public int Index { get; set; }

        [DataMember]
        public double PointX { get; set; }

        [DataMember]
        public double PointY { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey(nameof(RoofPlaneObstructionID))]
        public RoofPlaneObstruction RoofPlaneObstruction { get; set; }

        #endregion

        public ObstructionPoint Clone(Guid roofPlaneObstructionID)
        {
            ObstructionPoint ret = (ObstructionPoint)this.MemberwiseClone();
            ret.Reset();
            ret.RoofPlaneObstructionID = roofPlaneObstructionID;
            ret.RoofPlaneObstruction = null;

            return ret;
        }
    }
}