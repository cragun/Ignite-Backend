using Newtonsoft.Json;

namespace DataReef.TM.Models.DTOs.Mortgage
{
    public class MortgageDetailItem
    {
        [JsonProperty("_Id")]
        public string Id { get; set; }

        [JsonProperty("_Score")]
        public decimal? Score { get; set; }

        [JsonProperty("_source")]
        public MortgageSource Source { get; set; }
    }
}
