using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The SeriesMeasure object holds one data point, such as before solar electricity in the first year.
    /// </summary>
    public class SeriesMeasure
    {
        /// <summary>
        /// The identifier of the series that that this data point belongs to.
        /// </summary>
        public string seriesId          { get; set; }

        /// <summary>
        /// Start date and time of this data point.
        /// </summary>
        public DateTime? fromDateTime   { get; set; }

        /// <summary>
        /// End date and time of this data point.
        /// </summary>
        public DateTime? toDateTime     { get; set; }

        /// <summary>
        /// The average rate of charge for this timeframe, denominated in the major currency applicable. 
        /// For instance, in USD this would be 0.12 for 12 cents.
        /// </summary>
        public decimal? rate            { get; set; }

        /// <summary>
        /// Quantity for this timeframe (usage or production etc typically in kWh).
        /// </summary>
        public decimal qty              { get; set; }

        /// <summary>
        /// Cost (or savings) of this timeframe.
        /// </summary>
        public decimal? cost            { get; set; }
    }
}
