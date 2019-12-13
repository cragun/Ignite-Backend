
namespace DataReef.TM.Models.DTOs.Solar.Genability.Enums
{
    public enum PVWatts5UsageProfilePropertyInputs
    {
        azimuth,

        //  System size in kW
        systemSize,

        //  Roof tilt in degrees
        tilt,

        moduleType,

        losses,

        inverterEfficiency,

        climateDataSearchRadius,

        DCACRatio,

        /// <summary>
        /// Ground Coverage Ratio
        /// </summary>
        gcr,

        climateDataset,
        trackMode,
    }
}
