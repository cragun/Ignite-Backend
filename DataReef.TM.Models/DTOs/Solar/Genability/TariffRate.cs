using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The tariff rate.
    /// </summary>
    public class TariffRate
    {
        /// <summary>
        /// Unique Genability ID (primary key) for each tariff rate.
        /// </summary>
        public string tariffRateId { get; set; }

        /// <summary>
        /// Associates the rate with a tariff (foreign key).
        /// </summary>
        public string tariffId { get; set; }

        /// <summary>
        /// Populated when this is a rider attached to this tariff. null otherwise.
        /// </summary>
        public string riderId { get; set; }

        /// <summary>
        /// Sequence of this rate in the tariff, for display purposes only (e.g. this is the order on the bill).
        /// </summary>
        public string tariffSequenceNumber { get; set; }

        /// <summary>
        /// Name of the group this rate belongs to.
        /// </summary>
        public string rateGroupName { get; set; }

        /// <summary>
        /// Name of this rate. Can be null (in which case use the group name).
        /// </summary>
        public string rateName { get; set; }

        /// <summary>
        /// If populated, this indicates the rates effective date is not the same as that of its tariff.
        /// </summary>
        public DateTime? fromDateTime { get; set; }

        /// <summary>
        /// If populated, this indicates the rates end date is not the same as that of its tariff.
        /// </summary>
        public DateTime? toDateTime { get; set; }

        /// <summary>
        /// Possible values:
        /// FIXED_PRICE - a fixed charge for the period
        /// CONSUMPTION_BASED - based on quantity used (e.g. kW/h)
        /// DEMAND_BASED - based on the peak demand (e.g. kW)
        /// QUANTITY - a rate per number of items (e.g. $5 per street light)
        /// FORMULA - a rate that has a specific or custom formula
        /// MINIMUM - a minimum amount that the LSE will charge you, regardless of the other charges
        /// TAX - a percentage tax rate which is applied to the sum of all of the other charges on a bill
        /// </summary>
        public string chargeType { get; set; }

        /// <summary>
        /// A comma separated string that indicates what class(es) of charges this rate is for. Values include TRANSMISSION, DISTRIBUTION, SUPPLY, TAX, and OTHER. 
        /// Also, this field indicates whether the rate is CONTRACTED (by-passable).
        /// </summary>
        public string chargeClass { get; set; }

        /// <summary>
        /// Indicates what period this charge is calculated for. This is usually the same as the billing period (and is usually monthly) but can be other intervals. Possible values are:
        /// MONTHLY - each calendar month
        /// DAILY - calculated for each day
        /// QUARTERLY - every 3 months
        /// ANNUALLY - every year
        /// </summary>
        public string chargePeriod { get; set; }

        /// <summary>
        /// Indicates whether this rate is BUY (no credit when supplying back), SELL (e.g. a feed in tariff), or NET (you get a credit, e.g. net metering).
        /// </summary>
        public string transactionType { get; set; }

        /// <summary>
        /// When not null, the property that defines the type of quantity this rate applies to.
        /// </summary>
        public string quantityKey { get; set; }

        /// <summary>
        /// When not null, the property that defines the eligibility criteria for this rate.
        /// </summary>
        public string applicabilityKey { get; set; }

        /// <summary>
        /// When populated this defines the variable which determines the upper limit(s) of this rate.
        /// </summary>
        public string variableLimitKey { get; set; }

        /// <summary>
        /// When not null, this is the name of the property that defines the variable rate. 
        /// In this case the rate field is null, or can (rarely) be used as an input to the determination of the variable rate.
        /// </summary>
        public string variableRateKey { get; set; }

        /// <summary>
        /// When not null, this is the name of the property that defines the variable factor to apply to this rate.
        /// </summary>
        public string variableFactorKey { get; set; }

        /// <summary>
        /// The rate bands.
        /// </summary>
        public List<TariffRateBand> rateBands { get; set; }
    }
}
