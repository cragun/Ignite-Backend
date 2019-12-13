using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class RoofPlaneDataView
    {
        public Guid RoofPlaneID { get; set; }

        public string Label { get; set; }

        public int Azimuth { get; set; }

        public double CenterX { get; set; }

        public double CenterY { get; set; }

        public double CenterLatitude { get; set; }

        public double CenterLongitude { get; set; }

        public string GenabilitySolarProviderProfileID { get; set; }

        public bool IsManuallyEntered { get; set; }

        public int ManuallyEnteredPanelsCount { get; set; }

        public double ModuleSpacing { get; set; }

        public double Pitch { get; set; }

        public int Racking { get; set; }

        public double RowSpacing { get; set; }

        public int Shading { get; set; }

        public double Tilt { get; set; }

        public int PanelsCount
        {
            get
            {
                return IsManuallyEntered ? ManuallyEnteredPanelsCount : Panels == null ? 0 : Panels.Count;
            }
        }

        public bool IsValid
        {
            get
            {
                const int MINIMUM_PLANE_POINTS = 3;
                return (IsManuallyEntered && ManuallyEnteredPanelsCount > 0) || (!IsManuallyEntered && Points != null && Points.Count >= MINIMUM_PLANE_POINTS);
            }
        }

        public IList<RoofPlanePointDataView> Points { get; set; }

        public IList<RoofPlaneEdgeDataView> Edges { get; set; }

        public IList<RoofPlanePanelDataView> Panels { get; set; }

        public IList<RoofPlaneObstructionDataView> Obstructions { get; set; }

        public SolarPanelDataView SolarPanel { get; set; }

        public InverterDataView Inverter { get; set; }
    }
}
