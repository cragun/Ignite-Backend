using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// Class containing CalculatedCost objects corresponding to each series. 
    /// They are indexed by seriesId, which you can find from the series collection.
    /// </summary>
    public class SeriesCosts
    {
        public CalculatedCost series1 { get; set; }

        public CalculatedCost series2 { get; set; }
    }
}
