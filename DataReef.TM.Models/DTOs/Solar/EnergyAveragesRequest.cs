using DataReef.TM.Models.DTOs.Solar.Genability;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar
{
    public class EnergyAveragesRequest
    {
        public double? MonthlyConsumption { get; set; }

        public double? MonthlyPrice { get; set; }

        public double? AnnualConsumption { get; set; }

        public double? AnnualPrice { get; set; }

        public DateTime StartDate { get; set; }

        [JsonIgnore]
        public double? Consumption => MonthlyConsumption.HasValue ? MonthlyConsumption.Value * 12 : AnnualConsumption ?? null;

        [JsonIgnore]
        public double? Price => MonthlyPrice.HasValue ? MonthlyPrice.Value * 12 : AnnualPrice ?? null;

        [JsonIgnore]
        public DateTime Start => new DateTime(StartDate.Year, StartDate.Month, 1);
        [JsonIgnore]
        public DateTime End => Start.AddYears(1);

        public List<ReadingData> ToReadingData()
        {
            if (!Consumption.HasValue)
            {
                return null;
            }

            return new List<ReadingData>{
                new ReadingData
                {
                    fromDateTime = Start.ToString("o"),
                    toDateTime = End.ToString("o"),
                    quantityUnit = "kWh",
                    quantityValue = $"{Consumption}"
                }
            };
        }
    }
}
