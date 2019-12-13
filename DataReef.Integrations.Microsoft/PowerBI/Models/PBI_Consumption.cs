using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Microsoft.PowerBI.Models
{
    public class PBI_Consumption : PBI_Base
    {
        public Guid PropertyID { get; set; }
        public Guid OUID { get; set; }

        /// <summary>
        /// unknown / iPhone / iPad / Web
        /// </summary>
        public string Source { get; set; }
        public double TotalWatts { get; set; }
        public double TotalCost { get; set; }
        public double AverageWatts { get; set; }
        public double AverageCost { get; set; }
        public int TotalConsumptionMonths { get; set; }
        public int ManuallyEnteredConsumptionMonths { get; set; }

    }

}
