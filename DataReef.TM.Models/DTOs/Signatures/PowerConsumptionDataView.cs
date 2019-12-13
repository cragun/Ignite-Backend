using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class PowerConsumptionDataView
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public decimal Watts { get; set; }

        public decimal Cost { get; set; }

        public PowerInformationSource? UsageSource { get; set; }

        public PowerInformationSource? CostSource { get; set; }

        public PowerConsumptionDataView() { }

        public PowerConsumptionDataView(SolarSystemPowerConsumption consumption)
        {
            Year = consumption.Year;
            Month = consumption.Month;
            Watts = consumption.Watts;
            Cost = consumption.Cost;
            UsageSource = consumption?.UsageSource;
            CostSource = consumption?.CostSource;
        }
    }
}
