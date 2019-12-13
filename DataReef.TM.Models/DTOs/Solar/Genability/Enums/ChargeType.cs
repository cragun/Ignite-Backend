using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability.Enums
{
    public enum ChargeType
    {
        /// <summary>
        /// A fixed charge for the period.
        /// </summary>
        FIXED_PRICE,

        /// <summary>
        /// Based on quantity used (e.g. kW/h).
        /// </summary>
        CONSUMPTION_BASED,

        /// <summary>
        /// Based on the peak demand (e.g. kW).
        /// </summary>
        DEMAND_BASED,

        /// <summary>
        /// A rate per number of items (e.g. $5 per street light).
        /// </summary>
        QUANTITY,

        /// <summary>
        /// A rate that has a specific or custom formula.
        /// </summary>
        FORMULA,

        /// <summary>
        /// A minimum amount that the LSE will charge you, regardless of the other charges.
        /// </summary>
        MINIMUM,
        
        /// <summary>
        /// A percentage tax rate which is applied to the sum of all of the other charges on a bill.
        /// </summary>
        TAX
    }
}
