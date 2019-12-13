using Newtonsoft.Json;

namespace DataReef.Integrations.Sunnova.Models.Request
{
    public class CreateLeadRequest
    {
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Street")]
        public string Street { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("PostalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("Mailing_Street__c")]
        public string Mailing_Street { get; set; }

        [JsonProperty("Mailing_City__c")]
        public string Mailing_City { get; set; }

        [JsonProperty("Mailing_State__c")]
        public string Mailing_State { get; set; }

        [JsonProperty("Mailing_PostalCode__c")]
        public string Mailing_PostalCode { get; set; }

        [JsonProperty("Preferred_Language__c")]
        public string Preferred_Language { get; set; } = "en";

        [JsonProperty("Phone")]
        public string Phone { get; set; }

    }
}
