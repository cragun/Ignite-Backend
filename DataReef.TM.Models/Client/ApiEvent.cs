using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Client
{
    [NotMapped]
    public class ApiEvent
    {
        public ApiEvent()
        {
            this.EventID = Guid.NewGuid();
            this.TimeStamp = System.DateTime.UtcNow;
            this.Properties = new Dictionary<string, string>();


        }

        public Guid EventID { get; set; }

        public string PrivateKey { get; set; }

        public System.DateTime TimeStamp { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EventDomain Domain { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EventAction Action { get; set; }

        public Guid ObjectID { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ApiObjectType ObjectType { get; set; }

        public Dictionary<string, string> Properties;
    }
}
