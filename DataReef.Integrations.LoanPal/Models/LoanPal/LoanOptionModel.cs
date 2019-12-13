using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.LoanPal.Models.LoanPal
{
    public class LoanOptionModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("rate")]
        public string Rate { get; set; }

        [JsonProperty("term")]
        public string Term { get; set; }

    }
}
