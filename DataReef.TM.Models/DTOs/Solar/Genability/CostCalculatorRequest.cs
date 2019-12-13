using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The request class for running a new cost calculation.
    /// </summary>
    public class CostCalculatorRequest
    {
        /// <summary>
        /// Starting date and time for this Calculate request. (Required)
        /// </summary>
        public string fromDateTime { get; set; }

        /// <summary>
        /// End date and time for this Calculate request. (Required)
        /// </summary>
        public string toDateTime { get; set; }

        /// <summary>
        /// The providerAccountId of the account for which to run this calculation. 
        /// This will cause the calculator to take default values from the specified account,
        /// but you can override values contained in the account by passing in values through the inputs array below. (Optional)
        /// </summary>
        public string providerAccountId { get; set; }

        /// <summary>
        /// Use an explicit Profile for this calculation. (Optional)
        /// </summary>
        public string profileId { get; set; }

        /// <summary>
        /// This field enables enforcing minimum charges on this calculation. (Optional, Default=false)
        /// </summary>
        public bool minimums { get; set; }

        /// <summary>
        /// Toggles the level of details for the calculation result. (Optional) Possible values are:
        /// ALL - return all details for this calculation (default)
        /// TOTAL - return only the overall total, without any details
        /// CHARGE_TYPE - group the details by charge types, such as FIXED, CONSUMPTION, QUANTITY
        /// RATE - group the details by rates. This is most similar to how a utility bill is constructed.
        /// </summary>
        public string detailLevel { get; set; }

        /// <summary>
        /// This controls how the calculation details are grouped, for example grouping the details by month or year. 
        /// There is no default value; if not specified the details will use the frequency contained within the rates. (Optional)
        /// MONTH - groups the calculation details by month.
        /// YEAR - groups the calculation details by year.
        /// </summary>
        public string groupBy { get; set; }

        /// <summary>
        /// A true or false flag. If the dates of the calculation represent an actual billing cycle, then you should set this to true. 
        /// This will give you precise values for items like fixed charges. 
        /// When it's not set, or set to false, these charges will be prorated across the number of days in the calculation. Default is false. (Optional)
        /// </summary>
        public string billingPeriod { get; set; }

        /// <summary>
        /// The input values to use when running the calculation. 
        /// This is where you would specify an accountId as well as any other inputs. Note: accountId can also be specified as a query string parameter, e.g. accountId=abc123. 
        /// If you are not specifying any inputs, you must still include an empty tariffInputs field. (Required)
        /// </summary>
        public List<PropertyData> tariffInputs { get; set; }

        /// <summary>
        /// The rate input values are used to override existing rates on the tariff during the calculation. 
        /// This enables modeling and/or setting customer specific rates during a calculation. (Optional)
        /// </summary>
        public List<TariffRate> rateInputs { get; set; }
    }
}
