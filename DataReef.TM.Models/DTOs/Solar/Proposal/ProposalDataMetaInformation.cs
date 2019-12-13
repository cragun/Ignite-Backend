using DataReef.TM.Models.DataViews.Geo;

namespace DataReef.TM.Models.DTOs.Solar.Proposal
{
    public class ProposalDataMetaInformation
    {
        public GeoPoint GenerateLocation { get; set; }
        public GeoPoint AcceptLocation { get; set; }
    }
}
