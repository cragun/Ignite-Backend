using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// A summary map of totalCost, kWh, and kW used in the calculation.
    /// </summary>
    public class CalculatedCostSummaryMap
    {
        public decimal? totalCost { get; set; }

        public decimal? kWh       { get; set; }

        public decimal? kW        { get; set; }
    }
}
