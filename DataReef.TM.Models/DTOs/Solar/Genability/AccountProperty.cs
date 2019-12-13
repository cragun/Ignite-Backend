using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// Each account property has a keyName (these correspond 1 to 1 with property values), and a dataValue.
    /// </summary>
    public class AccountProperty:BaseResponse
    {
        /// <summary>
        /// The unique name of this key, e.g. customerClass.
        /// </summary>
        public string keyName         { get; set; }

        /// <summary>
        /// The value of this account property.
        /// </summary>
        public string dataValue       { get; set; }

        /// <summary>
        /// The data type of the value.
        /// </summary>
        public string dataType        { get; set; }

        /// <summary>
        /// The accuracy of the value.
        /// </summary>
        public string accuracy        { get; set; }

        /// <summary>
        /// The optional starting date and time of this property, if applicable.
        /// </summary>
        public DateTime? fromDateTime { get; set; }

        /// <summary>
        /// The optional end date and time of this property, if applicable.
        /// </summary>
        public DateTime? toDateTime   { get; set; }
    }
}