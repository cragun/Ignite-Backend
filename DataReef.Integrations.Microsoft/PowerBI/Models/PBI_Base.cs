using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Microsoft.PowerBI.Models
{
    public abstract class PBI_Base
    {
        private static string _env = ConfigurationManager.AppSettings["DataReef.Environment"];
        public PBI_Base()
        {
            Environment = _env;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Dev / Stage or Live
        /// </summary>
        [JsonProperty("Environment")]
        public string Environment { get; private set; }

        [JsonProperty("Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
