using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The savings analysis endpoint is very flexible. 
    /// From this single endpoint, you can configure your analysis in thousands of different ways. 
    /// You can change your account's tariff to a different one in the "after" scenario, you can change how quickly electricity rates go up each year, or you can use an entirely different solar power system than the one that you proposed. 
    /// To enable all of that flexibility, you have a lot of different parameters to chose from. 
    /// They are split into three groups: top level parameters that set the stage for the entire analysis, property inputs that define the parameters of each scenario, and rate inputs that let you specify entirely new rates to apply to your analysis.
    /// </summary>
    public class SavingsAnalysisRequest
    {
        /// <summary>
        /// The Genability-generated ID of the account for which you want to do the savings analysis.
        /// </summary>
        public string accountId                                 { get; set; }

        /// <summary>
        /// The alternative ID of the account for which you want to do the savings analysis.
        /// </summary>
        public string providerAccountId                         { get; set; }

        /// <summary>
        /// The start of your analysis. This setting affects the version of the account's tariffs that are in effect during the first year of the analysis. 
        /// You can use DateTime.now() or simliar to force the use of the most recent version of your account's tariffs.
        /// </summary>
        public string fromDateTime                              { get; set; }

        /// <summary>
        /// Sets the duration of the intial period of the analysis. The default is one year.
        /// </summary>
        public string toDateTime                                { get; set; }

        /// <summary>
        /// The parameters for the analysis. There are many different options available.
        /// </summary>
        public List<SavingAnalysisPropertyData> propertyInputs  { get; set; }

        /// <summary>
        /// Custom tariffs for this analysis. You can use this to define lease rates, PPA rates, or special tax rates. 
        /// Make sure to set the scenario property so that your rate is applied correctly.
        /// </summary>
        public List<TariffRate> rateInputs                      { get; set; }

        /// <summary>
        /// If set to true, your results will contain an even more detailed breakdown of the first year of results for this calculation.
        /// </summary>
        public bool populateCosts                               { get; set; }

        /// <summary>
        /// Previously, running a Savings Analysis required a full year of usage coverage. 
        /// With Intelligent Baselining on, you can use as little as one monthly bill to estimate usage. 
        /// (Using two or more months of billing data if available will increase accuracy.) 
        /// </summary>
        public bool useIntelligentBaselining                    { get; set; }
    }
}
