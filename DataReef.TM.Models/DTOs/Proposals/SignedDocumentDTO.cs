using DataReef.TM.Models.DataViews.Geo;
using System;

namespace DataReef.TM.Models.DTOs.Proposals
{
    public class SignedDocumentDTO
    {

    
        public float Apr { get; set; }
        public float Year { get; set; }
        public string ProviderName { get; set; }
       

        public string Name { get; set; }
        public Guid? ProposalDataID { get; set; }
        public string Url { get; set; }
        public string EnergyBillUrl { get; set; }

        public string PDFUrl { get; set; }
        public string Description { get; set; }
        public GeoPoint AcceptLocation { get; set; }
    }
}
