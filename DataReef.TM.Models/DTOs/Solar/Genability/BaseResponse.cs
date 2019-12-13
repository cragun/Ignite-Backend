using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    public abstract class BaseResponse
    {
        #region error response properties

        /// <summary>
        /// Error code. Unique string identifying the type of error. See the table below for a list of possible error codes. (Always returned)
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Property used to get the reason when the request fails
        /// Localized user readable message describing the error. (Conditionally returned)
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Identifies the type of object that the error is related to. 
        /// This is typically a resource type that is part of the response. For example, the Tariff object. (Conditionally returned)
        /// </summary>
        public string objectName { get; set; }

        /// <summary>
        /// Identifies the property of the object that the error is related to. 
        /// Primarily for binding and validation errors but sometimes used for business logic errors too. 
        /// For example, the tariffId or lseId. (Conditionally returned)
        /// </summary>
        public string propertyName { get; set; }

        public string propertyValue { get; set; }
        #endregion
    }
}
