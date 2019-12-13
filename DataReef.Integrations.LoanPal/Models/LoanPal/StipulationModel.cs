using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.LoanPal.Models.LoanPal
{
    public class StipulationModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("internal")]
        public bool Internal { get; set; }

    }
}
