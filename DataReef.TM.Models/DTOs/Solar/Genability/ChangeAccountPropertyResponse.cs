using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The response class for a change account property request.
    /// </summary>
    public class ChangeAccountPropertyResponse
    {
        /// <summary>
        /// The response status.
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// The results count.
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// The response type.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// The returned tariffs.
        /// </summary>
        public List<AccountProperty> results { get; set; }
    }
}
