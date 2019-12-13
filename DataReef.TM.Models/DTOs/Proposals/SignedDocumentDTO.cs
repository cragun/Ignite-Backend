using DataReef.TM.Models.DataViews.Geo;
using System;

namespace DataReef.TM.Models.DTOs.Proposals
{
    public class SignedDocumentDTO
    {
        public string Name { get; set; }
        public Guid? ProposalDataID { get; set; }
        public string Url { get; set; }
        public string EnergyBillUrl { get; set; }

        public string PDFUrl { get; set; }
        public string Description { get; set; }
        public GeoPoint AcceptLocation { get; set; }
    }
}
