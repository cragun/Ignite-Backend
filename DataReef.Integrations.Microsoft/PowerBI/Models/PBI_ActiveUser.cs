using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Microsoft.PowerBI.Models
{
    public class PBI_ActiveUser : PBI_Base
    {
        [JsonProperty("UserID")]
        public Guid UserID { get; set; }

        [JsonProperty("DeviceID")]
        public Guid DeviceID { get; set; }

    }
}
