using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataReef.TM.Models.DTOs.Mortgage
{
    public class MortgageDetail
    {
        public int Total { get; set; }

        [JsonProperty("Max_Score")]
        public decimal? MaxScore { get; set; }

        [JsonProperty("Hits")]
        public List<MortgageDetailItem> Items { get; set; }
    }
}
