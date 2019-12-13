using Newtonsoft.Json;

namespace DataReef.Integrations.Core.Models
{
    [JsonObject(Title = "System")]
    public class SystemElement
    {
        public string InverterManufacturer { get; set; }

        public string InverterQuantity     { get; set; }

        public string ModuleId             { get; set; }

        public string ModuleQuantity       { get; set; }

        public string MountingType         { get; set; }

        public string PanelSize            { get; set; }

        public string totalCost            { get; set; }

        public string InverterType         { get; set; }
    }
}
