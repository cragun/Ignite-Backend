using DataReef.TM.Models.Solar;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class RoofPlanePanelDataView
    {
        public RoofPlanePanelDataView(RoofPlanePanel roofPlanePanel)
        {
            if (roofPlanePanel == null) return;

            CenterX = roofPlanePanel.CenterX;
            CenterY = roofPlanePanel.CenterY;
            TopLeftLat = roofPlanePanel.TopLeftLat;
            TopLeftLon = roofPlanePanel.TopLeftLon;
            BottomRightLat = roofPlanePanel.BottomRightLat;
            BottomRightLon = roofPlanePanel.BottomRightLon;
            Width = roofPlanePanel.Width;
            Height = roofPlanePanel.Height;
        }

        public double CenterX { get; set; }

        public double CenterY { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public double TopLeftLat { get; set; }

        public double TopLeftLon { get; set; }

        public double BottomRightLat { get; set; }

        public double BottomRightLon { get; set; }
    }
}
