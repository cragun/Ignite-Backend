using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    /// <summary>
    /// Class used to capture Production and Price per KW information for a roof plane
    /// </summary>
    public class ProductionPrice
    {
        public decimal Production { get; set; }
        public decimal PricePerWatt { get; set; }
    }
}
