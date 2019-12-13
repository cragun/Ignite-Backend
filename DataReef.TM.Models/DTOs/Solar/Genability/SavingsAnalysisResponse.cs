using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The data that you get back from the savings analysis represents a complete description of the results of your calculation.
    /// </summary>
    public class SavingsAnalysisResponse
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
        /// The returned account analysis.
        /// </summary>
        public List<AccountAnalysis> results { get; set; }
    }
}
