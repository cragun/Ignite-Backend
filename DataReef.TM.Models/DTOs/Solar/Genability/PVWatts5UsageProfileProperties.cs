
namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    public class PVWatts5UsageProfileProperties
    {
        /// <summary>
        /// The azimuth of the array. 0 = Due north, 180 = Due south.
        /// </summary>
        public PropertyData azimuth { get; set; }

        /// <summary>
        /// The desired system size, in kW.
        /// </summary>
        public PropertyData systemSize { get; set; }

        /// <summary>
        /// The tilt of the array. 0 = Flat, 90 = Vertical.
        /// </summary>
        public PropertyData tilt { get; set; }


        /// <summary>
        /// (PVWatts v5 only) The type of module to use. 0 = Standard, 1 = Premium, 2 = Thin Film.
        /// </summary>
        public PropertyData moduleType { get; set; }


        /// <summary>
        /// (PVWatts v5 only) Losses in the system other than the inverter. 
        /// Must be between -5 and 99. 0 = no energy lost, 99 = 1% of energy remaining.
        /// </summary>
        public PropertyData losses { get; set; }


        /// <summary>
        /// (PVWatts v5 only) The efficiency of your inverter. 
        /// Must be between 90 and 99.5. 90 = 10% of energy lost, 99.5 = 0.5% of energy lost.
        /// </summary>
        public PropertyData inverterEfficiency { get; set; }

        /// <summary>
        /// The search radius to use when searching for the closest climate data station (miles). Pass in radius=0 to use the closest station regardless of the distance.
        /// </summary>
        public PropertyData climateDataSearchRadius { get; set; }

        /// <summary>
        /// DC to AC size Ratio = Calculated by: DC System size / inverter's AC rated size
        /// </summary>
        public PropertyData DCACRatio { get; set; }

        /// <summary>
        /// Ground Coverage Ratio = .4
        /// </summary>
        public PropertyData gcr { get; set; }

        /// <summary>
        /// The climate data set that you want to use. Valid options are “tmy2”, “tmy3”, and “intl”. Default: tmy2.
        /// </summary>
        public PropertyData climateDataset { get; set; }

        /// <summary>
        /// Set the racking type of the array.
        /// 0 = Fixed - Roof Mounted(Fixed – PVWatts v4)
        /// 1 = Single Axis Backtracking(Single Axis – PVWatts v4)
        /// 2 = Double Axis
        /// 3 = Fixed - Open Rack(Fixed – PVWatts v4)
        /// 4 = Single Axis
        /// </summary>
        public PropertyData trackMode { get; set; }
    }
}
