using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.FinancialIntegration.LoanPal
{
    public class LoanPalErrorModel
    {
        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("dataPath")]
        public string DataPath { get; set; }

        [JsonProperty("schemaPath")]
        public string SchemaPath { get; set; }

        [JsonProperty("params")]
        public Dictionary<string, string> Params { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

    }
}
