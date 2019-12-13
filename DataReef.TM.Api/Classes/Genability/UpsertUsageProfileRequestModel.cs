using DataReef.TM.Models.DTOs.Solar;
using DataReef.TM.Models.DTOs.Solar.Genability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.Genability
{
    public class UpsertUsageProfileRequestModel
    {
        public List<EnergyMonthDetails> Consumptions      { get; set; }

        public string ProfileName                   { get; set; }

        public bool? GenerateSlope                  { get; set; }
    }
}