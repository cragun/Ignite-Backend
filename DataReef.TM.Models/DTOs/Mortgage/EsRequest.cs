using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataReef.TM.Models.DTOs.Mortgage
{
    public class EsRequest
    {
        [JsonProperty("sort")]
        public IList<EsSort> Sort { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("query")]
        public EsQuery Query { get; set; }

        public static EsRequest FromMortgageRequest(MortgageRequest request, string pageSize)
        {
            var esRequest = new EsRequest
            {
                Sort = new List<EsSort> { new EsSort { RecordingDate = new EsRecordingDate() } },
                Size = pageSize,
                Query = new EsQuery
                {
                    Bool = new EsBool
                    {
                        Filter = new List<EsFilter>
                        {
                            new EsFilter {Term = new EsTerm {ZipCode = request.ZipCode}},
                            new EsFilter {Term = new EsTerm {ZipPlusFour = request.ZipPlusFour}},
                            new EsFilter {Term = new EsTerm {HouseNumber = request.HouseNumber}},
                            new EsFilter {Term = new EsTerm {FirstMortgageDetailedDocumentType = "MG"}},
                        }
                    }
                }
            };

            return esRequest;
        }
    }

    public class EsRecordingDate
    {
        [JsonProperty("order")]
        public string Order => "desc";
    }

    public class EsSort
    {
        public EsRecordingDate RecordingDate { get; set; }
    }

    public class EsTerm
    {
        public string ZipCode { get; set; }

        public string ZipPlusFour { get; set; }

        public string HouseNumber { get; set; }

        public string FirstMortgageDetailedDocumentType { get; set; }
    }

    public class EsFilter
    {
        [JsonProperty("term")]
        public EsTerm Term { get; set; }
    }

    public class EsBool
    {
        [JsonProperty("filter")]
        public IList<EsFilter> Filter { get; set; }
    }

    public class EsQuery
    {
        [JsonProperty("bool")]
        public EsBool Bool { get; set; }
    }
}
