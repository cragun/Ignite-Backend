using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SST
{
    [DataContract]
    public class SstRequestLead
    {
        [DataMember(Name = "sales_owner")]
        public string SalesOwner { get; set; }

        [DataMember(Name = "associated_id")]
        [JsonProperty("associated_id")]
        public long AssociatedID { get; set; }
    }
}