using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataReef.TM.Api.Areas.HelpPage.Models
{
    /// <summary>
    /// The model describing basic endpoint information
    /// </summary>
    public class HelpPageApiResourceModel
    {
        /// <summary>
        /// The name of the endpoint
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Simple controller description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// API URL 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The type of resource.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public HelpPageApiResourceType Type { get; set; }
    }
}