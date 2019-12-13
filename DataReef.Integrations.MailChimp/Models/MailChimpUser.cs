using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DataReef.Integrations.MailChimp.Models
{
    [DataContract]
    public class MailChimpUser
    {
        [DataMember]
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [DataMember]
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}