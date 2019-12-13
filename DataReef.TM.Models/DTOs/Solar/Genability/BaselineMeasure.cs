using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// These are simply an index (i) which increases in chronological order from 1 to 8760 (if hourly) or 4 x 8760 (if 15 minutes), and a value (v) that denotes that intervals value.
    /// </summary>
    public class BaselineMeasure
    {
        /// <summary>
        /// Interval of the year (hour or month).
        /// </summary>
        public int i        { get; set; }

        /// <summary>
        /// Amount (value) for this interval.
        /// </summary>
        public decimal v    { get; set; }
    }
}
