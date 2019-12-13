using DataReef.TM.Models.DataViews.Geo;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard
{
    public class SBProposalAttachRequest
    {
        public SBProposalAttachRequest(Proposal proposal)
        {
            EnergyUsage = new SBEnergyUtilityModel(proposal);
            MonthlyUsage = new SBUsageModel(proposal?.SolarSystem);
            if (proposal?.SolarSystem != null)
            {
                LeadKwh = new SBLeadKwhModel(proposal?.SolarSystem);
            }
        }

        public SBLeadModel Lead { get; set; }
        public SBLeadKwhModel LeadKwh { get; set; }
        public SBProposalModel Proposal { get; set; }
        public SBProposalDataModel ProjectData { get; set; }


        public SBEnergyUtilityModel EnergyUsage { get; set; }

        public SBUsageModel MonthlyUsage { get; set; }
    }

    public class SBProposalModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("energyBill_url")]
        public string EnergyBillUrl { get; set; }

        [JsonProperty("signedDate")]
        public DateTime? SignedDate { get; set; }

        [JsonProperty("signedLocation")]
        public GeoPoint SignedLocation { get; set; }
    }

}
