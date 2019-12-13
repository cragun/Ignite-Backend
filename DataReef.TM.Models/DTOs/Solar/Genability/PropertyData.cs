using DataReef.TM.Models.DTOs.Solar.Genability.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// Property data and tariff inputs are closely related. 
    /// Property data tells you the properties associated with the calculation of this tariff. 
    /// Tariff inputs are the populated properties you pass in when running a calculation.
    /// </summary>
    public class PropertyData
    {
        public PropertyData() { }

        public PropertyData(PVWatts5UsageProfilePropertyInputs key, string value)
        {
            keyName = key.ToString();
            dataValue = value;
        }

        /// <summary>
        /// The key name of the property associated with this input. 
        /// The most common one will be consumption (which is the kWh for the period), and second most common is demand (kW), but can also be applicability properties like cityLimits or hasElectricVehicle.
        /// </summary>
        public string keyName { get; set; }

        /// <summary>
        /// The value of this Property.
        /// </summary>
        public string dataValue { get; set; }

        /// <summary>
        /// The type of this property. Possible values are: "STRING", "CHOICE", "BOOLEAN", "DATE", "DECIMAL", "INTEGER", and "FORMULA".
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// The start date and time of this calculation.
        /// </summary>
        public string fromDateTime { get; set; }

        /// <summary>
        /// The end date and time of this calculation.
        /// </summary>
        public string toDateTime { get; set; }

        /// <summary>
        /// Where applicable, this is the unit of the value. Most common are: "kWh" - for keys of consumption, "kW" - for keys of demand.
        /// </summary>
        public string unit { get; set; }
    }
}
