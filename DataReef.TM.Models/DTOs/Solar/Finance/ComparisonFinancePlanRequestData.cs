using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class ComparisonFinancePlanRequestData
    {
        public Guid Guid { get; set; }
        public decimal DealerFee { get; set; }
        /// <summary>
        /// Finance Plan specific Additional data as JSON
        /// </summary>
        public string Data { get; set; }
    }
}
