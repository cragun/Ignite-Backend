using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    public class UsageProfileRequest
    {

        public UsageProfileRequest()
        {

        }

        public UsageProfileRequest(string accountId, EnergyAveragesRequest req)
        {
            ProviderAccountId = accountId;
            ServiceTypes = "ELECTRICITY";
            SourceId = "ReadingEntry";
            IsDefault = true;
            ReadingData = req.ToReadingData();
        }

        /// <summary>
        /// Provider account id.
        /// </summary>
        [JsonProperty("providerAccountId")]
        public string ProviderAccountId { get; set; }

        [JsonProperty("serviceTypes")]
        public string ServiceTypes { get; set; }

        [JsonProperty("sourceId")]
        public string SourceId { get; set; }

        /// <summary>
        /// Denotes whether this is the default profile for the service type. Default profiles are used for calcs when no profile is specified.
        /// </summary>
        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// The reading data.
        /// </summary>
        [JsonProperty("readingData")]
        public List<ReadingData> ReadingData { get; set; }
    }
}
