using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Genability.Utils
{
    /// <summary>
    /// Class encapsulating the Genability endpoint addresses.
    /// </summary>
    public static class Endpoints
    {
        public static readonly string createAccountResource                 = @"/rest/v1/accounts";

        public static readonly string getSmartPriceResource                 = @"/rest/v1/prices/smart";

        public static readonly string getAccountTariffsResource             = @"/rest/v1/accounts/pid/{0}/tariffs";

        public static readonly string changeAccountPropertyResource         = @"/rest/v1/accounts/pid/{0}/properties";

        public static readonly string getLSEsResource                       = @"/rest/public/lses";

        public static readonly string getTariffsResource                    = @"/rest/public/tariffs";

        public static readonly string getAccountResource                    = @"/rest/v1/accounts/pid/{0}";

        public static readonly string calculateCostResource                 = @"/rest/v1/calculate/{0}";

        public static readonly string calculateCostGetConsumptionResource   = @"/rest/v1/calculate";

        public static readonly string upsertUsageProfileResource            = @"/rest/v1/profiles";

        public static readonly string getUsageProfileResource               = @"/rest/v1/profiles/{0}";

        public static readonly string getUsageProfileForProviderProfileIdResource = @"/rest/v1/profiles/pid/{0}";

        public static readonly string calculateSavingAnalysisResource       = @"/rest/v1/accounts/analysis";

        public static readonly string calculateUsageForProviderAccountResource = @"/rest/v1/accounts/pid/{0}/calculate/";


    }
}
