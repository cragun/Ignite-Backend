using DataReef.TM.Models.DataViews.Geo;
using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Proposals
{
    public class ProposalWarrenties
    { 
        public string Name { get; set; }
        public string Value { get; set; }


        public static List<ProposalWarrenties> GetWarrenties()
        {
            return new List<ProposalWarrenties> { new ProposalWarrenties { Name = "Panels", Value = "25 YEAR WARRANTY" },
                new ProposalWarrenties { Name = "Panels", Value = "25 YEAR WARRANTY" },
                new ProposalWarrenties { Name = "Inverters", Value = "25 YEAR WARRANTY" },
                new ProposalWarrenties { Name = "Racking & Roof Penetration", Value = "25 YEAR WARRANTY" },
                new ProposalWarrenties { Name = "Workmanship", Value = "25 YEAR WARRANTY" },
                new ProposalWarrenties { Name = "Production Guarantee", Value = "25 YEAR WARRANTY" },
                new ProposalWarrenties { Name = "Monitoring", Value = "25 YEAR WARRANTY" }
            };
        }
    }

  
}
