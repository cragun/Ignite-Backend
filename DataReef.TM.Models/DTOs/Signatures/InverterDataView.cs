using DataReef.TM.Models.Solar;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class InverterDataView
    {
        public InverterDataView(Inverter inverter)
        {
            if (inverter == null) return;

            IsMicroInverter = inverter.IsMicroInverter;
            Model = inverter.Model;
            Manufacturer = inverter.Manufacturer;
            Efficiency = inverter.Efficiency;
            MaxSystemSizeInWatts = inverter.MaxSystemSizeInWatts;
        }

        public bool IsMicroInverter { get; set; }

        public string Model { get; set; }

        public string Manufacturer { get; set; }

        public double Efficiency { get; set; }

        public int MaxSystemSizeInWatts { get; set; }
    }
}
