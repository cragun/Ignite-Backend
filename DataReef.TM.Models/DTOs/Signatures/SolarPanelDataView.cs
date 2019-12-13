using DataReef.TM.Models.Solar;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class SolarPanelDataView
    {
        public SolarPanelDataView(SolarPanel solarPanel)
        {
            if (solarPanel == null) return;

            Description = solarPanel.Description;
            Watts = solarPanel.Watts;
            Width = solarPanel.Width;
            Height = solarPanel.Height;
            Thickness = solarPanel.Thickness;
            Weight = solarPanel.Weight;
            NumberOfCells = solarPanel.NumberOfCells;
            ModuleType = solarPanel.ModuleType;
            Efficiency = solarPanel.Efficiency;
        }

        public string Description { get; set; }

        public int Watts { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public double Thickness { get; set; }

        public double Weight { get; set; }

        public int NumberOfCells { get; set; }

        public int ModuleType { get; set; }

        public double Efficiency { get; set; }
    }
}
