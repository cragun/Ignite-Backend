using DataReef.TM.Models.Solar;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Solar.Proposal
{
    public class SolarProposalRequest
    {

        /// <summary>
        /// From Genability
        /// </summary>
        public string TariffID { get; set; }


        /// <summary>
        /// trailing 12 months consumption rates in watts
        /// </summary>
        public ICollection<EnergyMonthDetails> Consumption { get; set; }

        /// <summary>
        /// trailing 12 months consumption rates in watts without reducing the consumption based on adders
        /// </summary>
        public ICollection<EnergyMonthDetails> OriginalConsumption { get; set; }

        public ICollection<AdderItem> ConsumptionReducingAdders { get; set; }

        /// <summary>
        /// Effenciency percent of the system.  Added by User
        /// </summary>
        public float Derate { get; set; }

        /// <summary>
        /// Size in Watts
        /// </summary>
        public int SystemSize { get; set; }


        public int Tilt { get; set; }

        public int Azimuth { get; set; }

        public List<RoofPlaneInfo> RoofPlanes { get; set; }

        /// <summary>
        /// Formatted Address stirng with Zip.   1082 Canyon View Ln.  Pleasant Grove, UT. 84062
        /// </summary>
        public string AddressString { get; set; }

        public ICollection<EnergyMonthDetails> ConsumptionWithAdders
        {
            get
            {
                var consumption = OriginalConsumption ?? Consumption;

                var adders = ConsumptionReducingAdders?
                                .Where(a => a.Type == Enums.AdderItemType.Adder
                                         && a.ReducesUsage)?
                                .ToList();
                if (adders?.Any() != true)
                {
                    return consumption;
                }

                var totalConsumption = (consumption?.Sum(c => c.Consumption) ?? 0) / 1000;
                if (totalConsumption == 0)
                {
                    return consumption;
                }
                var totalReduction = adders.Sum(a => a.GetAdderUsageReduction(totalConsumption));

                var percentage = totalReduction * 100 / totalConsumption;

                return consumption.Select(c => c.Clone(percentage)).ToList();
            }
        }

    }
}
