using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// Class that extends TariffRate.
    /// </summary>
    public class SavingAnalysisTariffRate: TariffRate
    {
        /// <summary>
        /// "scenarios":"before,after". 
        /// Think of a scenario as one service that your customer pays for (like electricity that they pay the utility for or solar that they pay a lease or PPA on). 
        /// A scenario therefor needs a tariff (rate plan) too. You need at least two scenarios otherwise there wouldn't be anything to compare to get savings.
        /// </summary>
        public string scenario { get; set; }
    }
}
