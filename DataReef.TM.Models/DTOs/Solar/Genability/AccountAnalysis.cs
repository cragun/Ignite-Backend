using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The Account Savings Analysis call returns an AccountAnalysis object when you run a calculation.
    /// </summary>
    public class AccountAnalysis : BaseResponse
    {
        /// <summary>
        /// This is null for Account Analysis (so ignore it). It is used in Project Analysis (different endpoint).
        /// </summary>
        public string designId { get; set; }

        /// <summary>
        /// Represents the status of the underlying data (profiles) that the analysis ran on. 
        /// 2 means all the underlying data is up to date. 
        /// 1 means its still processing, and you should run the analysis again once the profile is done (this is very unusual).
        /// </summary>
        public int dataStatus { get; set; }

        /// <summary>
        /// Account analysis summary.
        /// </summary>
        public AccountAnalysisSummary summary { get; set; }


        /// <summary>
        /// Each scenario in the analysis. 
        /// For a solar PV analysis there will be a "before" for without solar electric bill, "after" for the with solar electric bill, "solar" for the solar system, and "savings" for net savings with solar (before - after + solar).
        /// </summary>
        public List<Scenario> scenarios { get; set; }

        /// <summary>
        /// A list of Series objects, each one describing the data points (SeriesMeasure) it contains. 
        /// The Series object is documented in further detail below.
        /// </summary>
        public List<Series> series { get; set; }

        /// <summary>
        /// This list holds the actual data points, each of type SeriesMeasure (described below). 
        /// Each ties to a particular series (above) using a unique integer value. 
        /// Each SeriesMeasure holds a rate, quantity, and cost.
        /// </summary>
        public List<SeriesMeasure> seriesData { get; set; }

        /// <summary>
        /// If populateCosts is set to true, this field is populated with CalculatedCost objects corresponding to each series. 
        /// They are indexed by seriesId, which you can find from the series collection.
        /// </summary>
        //public SeriesCosts seriesCosts          { get; set; }
        public List<CalculatedCost> seriesCosts { get; set; }

    }
}
