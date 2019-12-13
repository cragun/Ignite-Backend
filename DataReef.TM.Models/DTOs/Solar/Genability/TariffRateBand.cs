using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The tariff rate band.
    /// </summary>
    public class TariffRateBand
    {
        /// <summary>
        /// Unique Genability ID (primary key) for each Band.
        /// </summary>
        public string tariffRateBandId { get; set; }


        /// <summary>
        /// ID of the rate this band belongs to (foreign key).
        /// </summary>                                     
        public string tariffRateId { get; set; }

        /// <summary>
        /// This bands position in the bands for its rate.
        /// </summary>
        public string rateSequenceNumber { get; set; }

        /// <summary>
        /// true indicates that this has banded consumption.
        /// </summary>   
        public bool? hasConsumptionLimit { get; set; }

        /// <summary>
        /// When hasConsumptionLimit is true, this indicates the upper consumption limit of this band. null means no upper limit.
        /// </summary>
        public decimal? consumptionUpperLimit { get; set; }

        /// <summary>
        /// true indicates that this has banded demand.
        /// </summary>
        public bool? hasDemandLimit { get; set; }

        /// <summary>
        /// When hasDemandLimit is true, this indicates the upper demand limit of this band. null means no upper limit.
        /// </summary>
        public decimal? demandUpperLimit { get; set; }

        /// <summary>
        /// true indicates that this has a limit based on a property.
        /// </summary>
        public bool? hasPropertyLimit { get; set; }

        /// <summary>
        /// When hasPropertyLimit is true, this indicates the upper limit of this band. null means no upper limit.
        /// </summary>
        public decimal? propertyUpperLimit { get; set; }

        /// <summary>
        /// When not null, this indicates the value of applicability property that qualifies for this rate.
        /// </summary>
        public string applicabilityValue { get; set; }

        /// <summary>
        /// A factor to be applied to the cost of the rate.
        /// </summary>
        public decimal? calculationFactor { get; set; }

        /// <summary>
        /// Charge amount for this band.
        /// </summary>
        public decimal? rateAmount { get; set; }

        /// <summary>
        /// Possible values:
        /// COST_PER_UNIT - rate amount multiplied by the number of units
        /// PERCENTAGE - percentage of a value (e.g. percentage of overall bill)
        /// </summary>
        public string rateUnit { get; set; }

        /// <summary>
        /// When true this band is a credit amount (reduces the bill).
        /// </summary>
        public bool? isCredit { get; set; }
    }
}
