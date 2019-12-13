using DataReef.TM.Models.DTOs.Solar;
using System.Collections.Generic;

namespace DataReef.TM.Api.Classes.Genability
{
    public class PresolarCostResponseModel
    {
        public List<EnergyMonthDetails> CalculateCostMonths { get; set; }

        public decimal AnnualTotalPrice { get; set; }

        public long AnnualTotalConsumption { get; set; }

        public decimal AvgUtilityCost => AnnualTotalConsumption == 0 ? 0 : AnnualTotalPrice / AnnualTotalConsumption;
    }
}