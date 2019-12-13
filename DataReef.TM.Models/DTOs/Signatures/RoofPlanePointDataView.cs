using DataReef.TM.Models.Solar;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class RoofPlanePointDataView
    {
        public RoofPlanePointDataView(RoofPlanePoint p)
        {
            if (p == null) return;

            Index = p.Index;
            Latitude = p.Latitude;
            Longitude = p.Longitude;
        }

        public int Index { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
