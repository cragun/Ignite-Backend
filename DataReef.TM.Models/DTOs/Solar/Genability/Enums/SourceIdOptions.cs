
namespace DataReef.TM.Models.DTOs.Solar.Genability.Enums
{
    public enum SourceIdOptions
    {
        /// <summary>
        /// If you want to have the integrated PVWatts engine create a solar profile for you, use a sourceId of PVWatts. This will default to PVWatts v4. If you want to use v5, you'll need to create a full Source object.
        /// </summary>
        PVWatts,

        /// <summary>
        /// When uploading any kind of usage data through the profile endpoint, you should use ReadingEntry.
        /// </summary>
        ReadingEntry,

        /// <summary>
        /// If you have any any other kind of solar simulation data that you want to upload through the profile endpoint, use SolarPvModel. If you have PVSyst or Helioscope CSVs, you can also use the File Uploader.
        /// </summary>
        SolarPvModel
    }
}
