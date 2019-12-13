using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Genability.Utils
{
    /// <summary>
    /// Class containing the hardcoded values passed in the Genability requests.
    /// </summary>
    public static class GenabilityDefaultParameterValues
    {
        public static readonly string solarDegradation      = "0.7";

        public static readonly string rateInflation         = "2.9";

        public static readonly int projectDuration          = 20;

        public static readonly decimal solarRateAmount      = 0.13m;

        public static readonly string losses                = "3";

        public static readonly string inverterEfficiency    = "97";

        public static readonly string PVWatts5SourceVersion = "5";


    }
}
