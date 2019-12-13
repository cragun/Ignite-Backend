using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Google.Models
{
    internal class ServiceAccountModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("project_id")]
        public string ProjectID { get; set; }

        [JsonProperty("private_key_id")]
        public string PrivateKeyID { get; set; }

        [JsonProperty("private_key")]
        public string PrivateKey { get; set; }

        [JsonProperty("client_email")]
        public string ClientEmail { get; set; }

        [JsonProperty("client_id")]
        public string ClientID { get; set; }

        [JsonProperty("auth_uri")]
        public string AuthURI { get; set; }

        [JsonProperty("token_uri")]
        public string TokenURI { get; set; }
        [JsonProperty("auth_provider_x509_cert_url")]
        public string AuthProviderURL { get; set; }
        [JsonProperty("client_x509_cert_url")]
        public string ClientCertURL { get; set; }

        public static ServiceAccountModel FromString(string value)
        {
            return JsonConvert.DeserializeObject<ServiceAccountModel>(value);
        }
    }
}
