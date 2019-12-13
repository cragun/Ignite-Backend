using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    ///  The Price object then contains an array of PriceChange objects. By default there is one for each actual change in price during the requested date range, or you can ask for prices grouped by "HOUR", "DAY", "MONTH" and "YEAR".
    /// </summary>
    public class Price:BaseResponse
    {
        /// <summary>
        /// Description of the actual or Smart Tariff.
        /// </summary>
        public string description             { get; set; }

        /// <summary>
        /// Unique Genability ID (primary key) for this tariff.
        /// </summary>
        public string masterTariffId          { get; set; }

        /// <summary>
        /// The starting date and time for this price summary.
        /// </summary>
        public DateTime? fromDateTime         { get; set; }

        /// <summary>
        /// The ending date and time for this price summary.
        /// </summary>
        public DateTime? toDateTime           { get; set; }

        /// <summary>
        /// The level of granularity that the price is returned in.
        /// </summary>
        public string detailLevel             { get; set; }

        /// <summary>
        /// The ISO currency code for which the prices are returned (e.g. USD).
        /// </summary>
        public string currency                { get; set; }

        /// <summary>
        /// The mean value of the rate within the specified time period.
        /// </summary>
        public decimal rateMean               { get; set; }

        /// <summary>
        /// The standard deviation of all the prices within the specified time period.
        /// </summary>
        public string rateStandardDeviation   { get; set; }

        /// <summary>
        /// An array containing details of all of the price changes within the specified time period.
        /// </summary>
        public List<PriceChange> priceChanges { get; set; }
    }
}
