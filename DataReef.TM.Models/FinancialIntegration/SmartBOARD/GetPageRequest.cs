using DataReef.TM.Models.Enums.SmartBOARD;
using System;

namespace DataReef.TM.Models.FinancialIntegration.SmartBOARD
{
    public class GetPageRequest
    {
        public Guid OUID { get; set; }
        public string Token { get; set; }
        public SBRedirectType? RedirectType { get; set; }
        public long? LeadID { get; set; }
    }
}
