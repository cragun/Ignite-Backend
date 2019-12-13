using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// Tariff properties represent metadata about a tariff.
    /// </summary>
    public class TariffProperty
    {
        /// <summary>
        /// Unique name for this property.
        /// </summary>
        public string keyName { get; set; }

        /// <summary>
        /// The display name of this property.
        /// </summary>
        public string displayName { get; set; }

        /// <summary>
        /// Top level categorization of the property hierarchy.
        /// </summary>
        public string keyspace { get; set; }

        /// <summary>
        /// Second level categorization of the property hierarchy, below keyspace.
        /// </summary>
        public string family { get; set; }

        /// <summary>
        /// A longer description of the tariff property. Good for further explanation as part of a customer "questionaire".
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// The data type of this property. Possible values are string, choice, boolean, date, decimal, integer, formula.
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// The category of property. Possible values are APPLICABILITY, RATE_CRITERIA, BENEFIT, DATA_REPUTATION.
        /// </summary>
        public string propertyTypes { get; set; }

        /// <summary>
        /// If applicable the specific value of this property.
        /// </summary>
        public string propertyValue { get; set; }

        /// <summary>
        /// If applicable the minimum value of this property.
        /// </summary>
        public string minValue { get; set; }

        /// <summary>
        /// If applicable the maximum value of this property.
        /// </summary>
        public string maxValue { get; set; }

        /// <summary>
        /// If this property is a FORMULA type, the formula details will be in this field.
        /// </summary>
        public string formulaDetail { get; set; }

        /// <summary>
        /// Whether the value of this Property is the default value.
        /// </summary>
        public bool? isDefault { get; set; }
    }
}
