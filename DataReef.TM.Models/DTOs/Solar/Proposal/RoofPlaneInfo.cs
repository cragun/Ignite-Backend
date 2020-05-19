using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DataReef.TM.Models.DTOs.Solar.Proposal
{
    public class RoofPlaneInfo
    {
        /// <summary>
        /// Size in Watts
        /// </summary>
        public decimal Size { get; set; }

        /// <summary>
        /// TargetOffset in percentage
        /// </summary>
        public int TargetOffset { get; set; }
        /// <summary>
        /// ArrayOffset in percentage
        /// </summary>
        public int ArrayOffset { get; set; }


        public int Tilt { get; set; }

        public int Azimuth { get; set; }

        /// <summary>
        /// Genability PVWatts5 Usage Profile name.
        /// </summary>
        public string ProfileName { get; set; }

        /// <summary>
        /// Genability PVWatts5 Usage Profile ID.
        /// </summary>
        public string ProviderProfileId { get; set; }

        private double _shading;

        public double Losses
        {
            get
            {
                return _shading;
            }
            set
            {
                _shading = value;
            }
        }

        public double Shading
        {
            get
            {
                return _shading;
            }
            set
            {
                _shading = value;
            }
        }

        public decimal InverterEfficiency { get; set; }

        public double PanelsEfficiency { get; set; }

        /// <summary>
        /// The type of module to use. 0 = Standard, 1 = Premium, 2 = Thin Film.
        /// This property is dynamically calculated based on <see cref="InverterEfficiency"/>
        /// Specs: 
        /// If efficiency is closer to 15% use 0, 
        /// If efficiency is closer to 19% use 1,
        /// </summary>
        public int ModuleType
        {
            get
            {
                if (PanelsEfficiency < 16)
                {
                    return 0;
                }
                if (PanelsEfficiency < 20)
                {
                    return 1;
                }
                return 2;
            }
        }

        public double GetLosses(Dictionary<string, ValueTypePair<SettingValueType, string>> ouSettings)
        {
            var lossesSetting = ouSettings.GetByKey<List<OrgSettingDataView>>(OUSetting.Solar_Losses);

            var losses = new List<double>() { Shading };

            var soiling = lossesSetting.GetValueAsDoubleByName(OUSetting.Solar_Losses_Soiling) ?? 2;
            losses.Add(soiling);

            var snow = lossesSetting.GetValueAsDoubleByName(OUSetting.Solar_Losses_Snow) ?? 0;
            losses.Add(snow);

            var mismatch = lossesSetting.GetValueAsDoubleByName(OUSetting.Solar_Losses_Mismatch) ?? 0;
            losses.Add(mismatch);

            var wiring = lossesSetting.GetValueAsDoubleByName(OUSetting.Solar_Losses_Wiring) ?? 2;
            losses.Add(wiring);

            var connections = lossesSetting.GetValueAsDoubleByName(OUSetting.Solar_Losses_Wiring) ?? 0.5;
            losses.Add(connections);

            // Light Induced Degradation
            var lid = lossesSetting.GetValueAsDoubleByName(OUSetting.Solar_Losses_LightInductedDegradation) ?? 1.5;
            losses.Add(lid);

            var nameplate = lossesSetting.GetValueAsDoubleByName(OUSetting.Solar_Losses_NamePlateRating) ?? 1;
            losses.Add(nameplate);

            var age = lossesSetting.GetValueAsDoubleByName(OUSetting.Solar_Losses_Age) ?? 0;
            losses.Add(age);

            var availability = lossesSetting.GetValueAsDoubleByName(OUSetting.Solar_Losses_Availability) ?? 0.5;
            losses.Add(availability);

            // formula:  100 * [ 1 - PROD ( 1 - loss[i] / 100 ) ]
            return 100 * Math.Abs(1 - losses.Select(l => 1 - l / 100).Aggregate(1d, (acc, val) => acc * val));
        }

    }
}
