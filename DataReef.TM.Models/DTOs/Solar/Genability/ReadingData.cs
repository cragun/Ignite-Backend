using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// ReadingData objects contain the point in time readings of a usage profile. 
    /// There are typically hundreds or thousands of reading data elements per usage profile.
    /// </summary>
    public class ReadingData
    {
        /// <summary>
        /// The type of data this reading represents. Typically will be 'consumption'.
        /// </summary>
        public string keyName               { get; set; }

        /// <summary>
        /// Date and time this reading starts.
        /// </summary>
        public string fromDateTime          { get; set; }

        /// <summary>
        /// Date and time this reading ends.
        /// </summary>
        public string toDateTime            { get; set; }

        /// <summary>
        /// Length of this interval.
        /// </summary>
        public decimal? duration            { get; set; }

        /// <summary>
        /// Value representing the accuracy, in ms, of the time reading.
        /// </summary>
        public int? timeAccuracy            { get; set; }

        /// <summary>
        /// Unit of this reading. Typically will be 'kWh'.
        /// </summary>
        public string quantityUnit          { get; set; }

        /// <summary>
        /// Value of this reading. e.g. the number of kWh.
        /// </summary>
        public string quantityValue         { get; set; }

        /// <summary>
        /// Indicator of the accuracy of the reading, in the same units as the quantityValue.
        /// </summary>
        public decimal? quantityAccuracy    { get; set; }

        /// <summary>
        /// One of OFF_PEAK, PARTIAL_PEAK, ON_PEAK, or CRITICAL_PEAK. Used for uploading TOU-based energy consumption data.
        /// </summary>
        public string touType               { get; set; }
    }
}
