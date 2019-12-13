using Newtonsoft.Json;

namespace DataReef.TM.Models.DTOs.Mortgage
{
    public class MortgageResponse
    {
        public int Took { get; set; }

        [JsonProperty("timed_out")]
        public bool TimedOut { get; set; }

        [JsonProperty("Hits")]
        public MortgageDetail Detail { get; set; }
    }
}
