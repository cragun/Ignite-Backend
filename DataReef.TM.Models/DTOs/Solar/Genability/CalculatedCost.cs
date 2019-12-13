using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The CalculatedCost object contains the summary of the calculation. 
    /// The totalCost field holds the overall cost while the details of the calculation are contained within a list of calculated cost items. 
    /// CalculatedCost also contains the list of inputs that were used in this calculation.
    /// </summary>
    public class CalculatedCost:BaseResponse
    {
        /// <summary>
        /// Unique Genability ID for the master tariff of this tariff.
        /// </summary>
        public string masterTariffId            { get; set; }

        /// <summary>
        /// The name of this tariff.
        /// </summary>
        public string tariffName                { get; set; }

        /// <summary>
        /// The start date and time of this calculation.
        /// </summary>
        public DateTime? fromDateTime           { get; set; }

        /// <summary>
        /// The end date and time of this calculation.
        /// </summary>
        public DateTime? toDateTime             { get; set; }

        /// <summary>
        /// Total summed up cost of all cost items (see below).
        /// </summary>
        public decimal? totalCost               { get; set; }

        /// <summary>
        /// A summary map of totalCost, kWh, and kW used in the calculation.
        /// </summary>
        public CalculatedCostSummaryMap summary { get; set; }

        /// <summary>
        /// A decimal value between 0 and 1 representing the accuracy of the calculation. 
        /// As more best guess calculation assumptions are made, the calculation will become less accurate and the accuracy field will be smaller.
        /// </summary>
        public decimal? accuracy                { get; set; }

        /// <summary>
        /// The assumptions that were used by the calculator. In the event that a required input was not provided, the calculator will make a best guess assumption for what was used. 
        /// The accuracy field of each PropertyData item indicates how accurate this input was, with values ranging from 0 - 1 with 1 being a completely accurate input.
        /// </summary>
        public List<PropertyData> assumptions   { get; set; }

        /// <summary>
        /// Array of the CalculatedCostItem objects that comprise this calculated cost. 
        /// These will be summarized as requested. Currently this is one item per tariff rate, but soon there will be other options for levels of summary or detail.
        /// </summary>
        public List<CalculatedCostItem> items   { get; set; }

    }
}
