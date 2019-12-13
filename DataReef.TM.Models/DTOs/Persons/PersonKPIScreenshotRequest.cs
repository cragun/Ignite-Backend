using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Persons
{
    public class PersonKPIScreenshotRequest
    {
        /// <summary>
        /// Base64 encoded value of the screenshot data used in personKPI
        /// </summary>
        public string ScreenshotData { get; set; }
    }
}
