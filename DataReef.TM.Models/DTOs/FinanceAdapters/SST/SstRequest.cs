using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SST
{
    [DataContract]
    public class SstRequest
    {
        [DataMember]
        public SstRequestLead Lead { get; set; }

        [DataMember]
        public SstRequestCustomer Customer { get; set; }

        [DataMember]
        public SstRequestEnergyUsage EnergyUsage { get; set; }

        [DataMember]
        public SstRequestLeadKwh LeadKwh { get; set; }

        [DataMember]
        public SstRequestProposal Proposal { get; set; }
    }
}
