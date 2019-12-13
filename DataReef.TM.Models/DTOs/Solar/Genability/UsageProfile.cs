using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// A usage profile organizes together one or more series' of usage data. 
    /// This data can include electricity consumption (kWh) or demand (kW). 
    /// It can take the form of bills or meter readings, and can be for electricity purchased from the utility or on-site solar energy. 
    /// They can also take into account usage reduction through energy efficiency. 
    /// Profiles belong to an Account object, and it is typical, although not required, to have at least one profile per account.
    /// </summary>
    public class UsageProfile : BaseResponse
    {
        /// <summary>
        /// Unique Genability ID (UUID) for this usage profile.
        /// </summary>
        public string profileId { get; set; }

        /// <summary>
        /// Optional alternate key.
        /// </summary>
        public string providerProfileId { get; set; }

        /// <summary>
        /// (Optional) Your specified name for this usage profile.
        /// </summary>
        public string profileName { get; set; }

        /// <summary>
        /// Required accountId for this usage profile. This is useful for linking usage profiles to your own customer Ids.
        /// </summary>
        public string accountId { get; set; }

        /// <summary>
        /// Provider account id.
        /// </summary>
        public string providerAccountId { get; set; }

        /// <summary>
        /// Optional description for this Usage Profile.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Comma separated strings of the types of service this profile denotes. Current types include "ELECTRICITY" and "SOLAR_PV".
        /// </summary>
        public string serviceTypes { get; set; }

        public string groupBy { get; set; }

        /// <summary>
        /// Optional. Source contains a sourceId and name (both strings). Uniquely identify the source of this profile. e.g. "PVWatts"
        /// </summary>
        public Source source { get; set; }


        /// <summary>
        /// You won't usually construct a full Source object when uploading a usage profile. Instead, you can just set the sourceId property. There are three options that you should choose from:
        /// ReadingEntry: When uploading any kind of usage data through the profile endpoint, you should use ReadingEntry.
        /// PVWatts: If you want to have the integrated PVWatts engine create a solar profile for you, use a sourceId of PVWatts. This will default to PVWatts v4. If you want to use v5, you'll need to create a full Source object.
        /// SolarPvModel: If you have any any other kind of solar simulation data that you want to upload through the profile endpoint, use SolarPvModel. If you have PVSyst or Helioscope CSVs, you can also use the File Uploader.
        /// </summary>
        public string sourceId { get; set; }

        /// <summary>
        /// Denotes whether this is the default profile for the service type. Default profiles are used for calcs when no profile is specified.
        /// </summary>
        public bool isDefault { get; set; }

        /// <summary>
        /// Our standard collection of PropertyData denoting properties specific to this Profile.
        /// </summary>
        public PVWatts5UsageProfileProperties properties { get; set; }

        /// <summary>
        /// Read-only value that Genability maintains to show how up-to-date the profile number crunching is. 
        /// 1=processing, 2= done, 3=error (we auto-fix 3)
        /// </summary>
        public int dataStatus { get; set; }

        /// <summary>
        /// Reading data summaries.
        /// </summary>
        public List<ReadingDataSummary> readingDataSummaries { get; set; }

        /// <summary>
        /// The intervals.
        /// </summary>
        //public List<IntervalInfo> intervals                     { get; set; }

        /// <summary>
        /// The reading data.
        /// </summary>
        public List<ReadingData> readingData { get; set; }

        /// <summary>
        /// (Optional) For estimated solar production, your profile doesn't necessarily have to correspond to a particular year. 
        /// You can use this field to input an array of generic datapoints corresponding to the hours of a year rather than specific dates.
        /// </summary>
        public List<BaselineMeasure> baselineMeasures { get; set; }
    }
}
