using DataReef.TM.Models.DTOs.Solar;
using DataReef.TM.Models.DTOs.Solar.Genability;
using DataReef.TM.Models.DTOs.Solar.Genability.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Genability.Utils
{
    public class UpsertUsageProfileModel
    {
        public string GenabilityAppID                       { get; set; }

        public string GenabilityAppKey                      { get; set; }

        public string ProviderAccountId                     { get; set; }

        public string ProviderProfileId                     { get; set; }

        public string ProfileName                           { get; set; }

        public string GroupBy                               { get; set; }

        public ServiceType ServiceType                      { get; set; }

        public string SourceId                              { get; set; }

        public List<EnergyMonthDetails> Months                    { get; set; }

        public Source Source                                { get; set; }

        public PVWatts5UsageProfileProperties Properties    { get; set; }        
    }
}
