using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.NetSuite
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
    }
}
