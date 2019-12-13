using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class RoofPlaneEdgeDataView
    {
        public RoofPlaneEdgeDataView(RoofPlaneEdge roofPlaneEdge)
        {
            if (roofPlaneEdge == null) return;

            Index = roofPlaneEdge.Index;
            EdgeType = roofPlaneEdge.EdgeType;
            EndPointLatitude = roofPlaneEdge.EndPointLatitude;
            EndPointLongitude = roofPlaneEdge.EndPointLongitude;
            FireOffsetIsEnabled = roofPlaneEdge.FireOffsetIsEnabled;
            FireOffset = roofPlaneEdge.FireOffset;
            StartPointLatitude = roofPlaneEdge.StartPointLatitude;
            StartPointLongitude = roofPlaneEdge.StartPointLongitude;
        }

        public RoofPanelEdgeType EdgeType { get; set; }

        public double StartPointLatitude { get; set; }

        public double StartPointLongitude { get; set; }

        public double EndPointLatitude { get; set; }

        public double EndPointLongitude { get; set; }

        public double FireOffset { get; set; }

        public bool FireOffsetIsEnabled { get; set; }

        public int Index { get; set; }
    }
}
