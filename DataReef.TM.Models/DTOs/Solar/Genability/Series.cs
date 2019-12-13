using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The Series object denotes the scenario, time frame, granularity of each data series in the results.
    /// </summary>
    public class Series
    {
        /// <summary>
        /// All the series measures (below) for this series have this identifier. 
        /// It is a unique integer within the response. You should not assume the integer will be the same for the same series between responses. 
        /// Instead look at the series' period, duration, and scenario fields to find the seriesId you need.
        /// </summary>
        public int seriesId             { get; set; }

        /// <summary>
        /// The start date of the series.
        /// </summary>
        public DateTime? fromDateTime   { get; set; }

        /// <summary>
        /// The end date of the series.
        /// </summary>
        public DateTime? toDateTime     { get; set; }

        /// <summary>
        /// Mathematical formula of scenarios that produced this series.
        /// </summary>
        public string scenario          { get; set; }

        /// <summary>
        /// The display label for this series.
        /// </summary>
        public string displayLabel      { get; set; }

        /// <summary>
        /// The series period/term. Possible values are: "YEAR", "MONTH", "DAY", and "HOUR".
        /// </summary>
        public string seriesPeriod      { get; set; }

        /// <summary>
        /// The duration of the series with respect to the seriesPeriod. e.g. if this is 20 and seriesPeriod is "YEAR", this series' data covers 20 years.
        /// </summary>
        public int seriesDuration       { get; set; }

        /// <summary>
        /// Not populated for Account Analysis.
        /// </summary>
        public string designId          { get; set; }

        /// <summary>
        /// For future use.
        /// </summary>
        public string key               { get; set; }

        /// <summary>
        /// Average rate over the entire duration of this series.
        /// </summary>
        public decimal rate             { get; set; }

        /// <summary>
        /// Total quantity over the entire duration of this series.
        /// </summary>
        public decimal qty              { get; set; }

        /// <summary>
        /// Total cost over the entire duration of this series.
        /// </summary>
        public decimal cost             { get; set; }

    }
}
