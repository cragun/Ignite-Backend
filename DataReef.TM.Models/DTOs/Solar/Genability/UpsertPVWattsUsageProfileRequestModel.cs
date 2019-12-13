
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.Enums;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    public class UpsertPVWattsUsageProfileRequestModel
    {
        public UpsertPVWattsUsageProfileRequestModel(Dictionary<string, ValueTypePair<SettingValueType, string>> ouSettings)
        {

        }

        public string ProfileName           { get; set; }

        /// <summary>
        /// Size in kW
        /// </summary>
        public string SystemSize            { get; set; }

        /// <summary>
        /// The tilt of the array. 0 = Flat, 90 = Vertical.
        /// </summary>
        public string Tilt                  { get; set; }

        /// <summary>
        /// The azimuth of the array. 0 = Due north, 180 = Due south.
        /// </summary>
        public string Azimuth               { get; set; }

        /// <summary>
        ///  Losses in the system other than the inverter. Must be between -5 and 99. 0 = no energy lost, 99 = 1% of energy remaining.
        /// </summary>
        public string Losses                { get; set; }

        /// <summary>
        /// The efficiency of your inverter. Must be between 90 and 99.5. 90 = 10% of energy lost, 99.5 = 0.5% of energy lost.
        /// </summary>
        public string InverterEfficiency    { get; set; }

        /// <summary>
        /// The type of module to use. 0 = Standard, 1 = Premium, 2 = Thin Film.
        /// </summary>
        public string ModuleType            { get; set; }
    }
}
