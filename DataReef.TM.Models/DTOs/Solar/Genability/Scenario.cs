using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// A Savings Analysis has at least two Scenario objects in its response. 
    /// For a solar PV analysis there are 4. 
    /// You only need to worry about sending this in a request to the Savings Calculator when running a generic analysis type.
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// This is not used for Account Analysis (it's used for Project Analysis) Will be null. Ignore it.
        /// </summary>
        public string id                { get; set; }
        
        /// <summary>
        /// The unique name (key) of this scenario. Solar PV has "before", "after", "solar" and "savings".
        /// </summary>
        public string name              { get; set; }

        /// <summary>
        /// The type of service for this scenario. "ELECTRICITY" or "SOLAR_PV".
        /// </summary>
        public string serviceType       { get; set; }

        /// <summary>
        /// The important inputs that were used in this scenarios calculation. 
        /// This defines the usage data profiles, the rate plan and plan arguments and other parameters for the calc.
        /// </summary>
        List<PropertyData> inputs       { get; set; }

        /// <summary>
        /// Used to pass the sites specific contracted electricity rates (only needed if they are in a deregulated market), 
        /// their tax rates (if you want us to calculate post tax numbers), and possibly their solar offers PPA or lease rate.
        /// </summary>
        public List<TariffRate> rates   { get; set; }
    }
}
