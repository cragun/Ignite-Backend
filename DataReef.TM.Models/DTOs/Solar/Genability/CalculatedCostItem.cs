using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The calculated cost items contain the details of the calculations. 
    /// There is one CalculatedCostItem object for each tariff rate that is included in the calculation.
    /// </summary>
    public class CalculatedCostItem
    {
        /// <summary>
        /// Unique Genability ID of the tariff rate that this charge.
        /// </summary>
        public string tariffRateId      { get; set; }

        /// <summary>
        /// Unique Genability ID for the band that this charge reached.
        /// </summary>
        public string tariffRateBandId  { get; set; }

        /// <summary>
        /// The name of the group this rate belongs to.
        /// </summary>
        public string rateGroupName     { get; set; }

        /// <summary>
        /// The name of this rate.
        /// </summary>
        public string rateName          { get; set; }

        /// <summary>
        /// The value of this rate.
        /// </summary>
        public decimal? rateAmount      { get; set; }

        /// <summary>
        /// The start date and time of the period applicable for this cost item.
        /// </summary>
        public DateTime? fromDateTime   { get; set; }

        /// <summary>
        /// The end date and time of the period applicable for this cost item.
        /// </summary>
        public DateTime? toDateTime     { get; set; }

        /// <summary>
        /// The type of rate this charge is. 
        /// Current values include "COST_PER_UNIT" which is a cost for each unit of quantity consumed (e.g. $0.10 per kWh), and "PERCENTAGE" which is a percent of another cost (e.g. 10% of your bill).
        /// </summary>
        public string rateType          { get; set; }

        /// <summary>
        /// The key for the quantity this calculated cost item refers to. Possible values include: fixed, consumption, minimum.
        /// </summary>
        public string quantityKey       { get; set; }

        /// <summary>
        /// Total quantity used for this item's charge. 
        /// This will typically be 1 but will have different values for charges related to the number of units of some items (e.g. number of billing meters at a facility).
        /// </summary>
        public decimal? itemQuantity    { get; set; }

        /// <summary>
        /// This is reserved for future use, to denote the count of items rolled up when getting the results summarized. This will typically be 1.
        /// </summary>
        public int? itemCount           { get; set; }

        /// <summary>
        /// This is the total cost for this line item.
        /// </summary>
        public decimal? cost            { get; set; }

        /// <summary>
        /// The accuracy of this item, between 0 and 1. Currently not populated.
        /// </summary>
        public decimal? accuracy        { get; set; }
    }
}
