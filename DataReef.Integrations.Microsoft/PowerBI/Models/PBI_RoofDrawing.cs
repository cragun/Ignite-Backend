using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Microsoft.PowerBI.Models
{
    public class PBI_RoofDrawing : PBI_Base
    {
        public Guid PropertyID { get; set; }
        public Guid OUID { get; set; }
        public Guid SalesRepID { get; set; }
        public string DeviceType { get; set; }
        public int RoofPlanesCount { get; set; }
        public string Panels { get; set; }
        public int PanelsCount { get; set; }
        public string Action { get; set; }

    }
}
