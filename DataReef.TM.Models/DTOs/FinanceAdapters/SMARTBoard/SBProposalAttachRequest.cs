﻿using DataReef.TM.Models.DataViews.Geo;
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
        public Contract contract { get; set; }
        public SBProposalDataModel ProjectData { get; set; }
        public SBEnergyUtilityModel EnergyUsage { get; set; }
        public SBUsageModel MonthlyUsage { get; set; }
    }
    public class Contract
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("extra_content")]
        public string ExtraContent { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }


    }

        public class SBProposalModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        //[JsonProperty("customer_name")]
        //public string CustomerName { get; set; }

        //[JsonProperty("module_qty")]
        //public string ModuleQty { get; set; }

        //[JsonProperty("panel_brand")]
        //public string PanelBrand { get; set; }

        //[JsonProperty("panel_size")]
        //public string PanelSize { get; set; }

        //[JsonProperty("system_size")]
        //public string SystemSize { get; set; }

        //[JsonProperty("lender")]
        //public string Lender { get; set; }

        //[JsonProperty("term_in_years")]
        //public string TermInYears { get; set; }

        //[JsonProperty("apr")]
        //public string Apr { get; set; }

        //[JsonProperty("year")]
        //public string Year { get; set; }

        //[JsonProperty("month")]
        //public string Month { get; set; }

        //[JsonProperty("day")]
        //public string Day { get; set; }

        //[JsonProperty("hour")]
        //public string Hour { get; set; }

        //[JsonProperty("minute")]
        //public string Minute { get; set; }

        [JsonProperty("proposal_name")]
        public string ProposalName { get; set; }

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
