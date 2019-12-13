using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class RoofPlaneObstructionDataView
    {
        public RoofPlaneObstructionDataView(RoofPlaneObstruction roofPlaneObstruction)
        {
            if (roofPlaneObstruction == null) return;

            ObstructionType = roofPlaneObstruction.ObstructionType;
            CenterPointX = roofPlaneObstruction.CenterPointX;
            CenterPointY = roofPlaneObstruction.CenterPointY;
            Radius = roofPlaneObstruction.Radius;
        }

        public ObstructionType ObstructionType { get; set; }

        public float? Radius { get; set; }

        public double? CenterPointX { get; set; }

        public double? CenterPointY { get; set; }
    }
}
