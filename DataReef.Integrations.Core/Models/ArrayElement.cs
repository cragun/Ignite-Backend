
namespace DataReef.Integrations.Core.Models
{
    public class ArrayElement
    {
        public string ArrayNumber         { get; set; }
                                          
        public string Azimuth             { get; set; }
                                          
        public string InverterId          { get; set; }
                                          
        public string InverterModel       { get; set; }

        public string ModuleQuantity      { get; set; }

        public string ModuleType          { get; set; }

        public string Orientation         { get; set; }

        public string[] Shading           { get; set; }

        public string[] MonthlyProduction { get; set; }

        public decimal SystemSize         { get; set; }

        public string Tilt                { get; set; }
    }
}
