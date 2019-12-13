using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// Price change within the specified time period.
    /// </summary>
    public class PriceChange
    {
        /// <summary>
        /// Name of this change. Usually something like the time of use period. For display purposes.
        /// </summary>
        public string name              { get; set; }

        /// <summary>
        /// Date and Time when this change begins. In ISO 8601 format.
        /// </summary>
        public DateTime? fromDateTime   { get; set; }

        /// <summary>
        /// Date and Time when this change ends. In ISO 8601 format.
        /// </summary>
        public DateTime? toDateTime     { get; set; }

        /// <summary>
        /// The actual charge amount for this price change.
        /// </summary>
        public string rateAmount        { get; set; }

        /// <summary>
        /// The mean delta of this price change compared to the overall mean across all price changes. 
        /// Good for understanding if this is a high or low price relative to the rest of the period.
        /// </summary>
        public string rateAmountDelta   { get; set; }
    }
}
