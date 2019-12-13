using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The profile has one ReadingDataSummary object for each series of data (a series is a unique quantityUnit). 
    /// These are read-only. 
    /// The system creates and updates them as you add and update reading data to your profile. 
    /// They provide a high level summary of the readings contained in each data series in this Profile, including the number of readings and the start and end dates of the reading range.
    /// </summary>
    public class ReadingDataSummary
    {
        public string quantityUnit      { get; set; }

        public long? numberOfReadings   { get; set; }

        public DateTime? firstStartTime { get; set; }

        public DateTime? lastEndTime    { get; set; }

    }
}
