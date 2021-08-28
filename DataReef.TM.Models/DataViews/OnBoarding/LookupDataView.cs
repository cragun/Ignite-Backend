using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.OnBoarding
{
    public class LookupDataView
    {
        public List<GuidNamePair> Panels { get; set; }
        public List<GuidNamePair> Inverters { get; set; }
        public List<GuidNamePair> Roles { get; set; }
        public List<FinancePlanDataView> FinancePlans { get; set; }
        public List<GuidNamePair> Templates { get; set; }
        public List<string> States { get; set; }
        public List<GuidNamePair> Ancestors { get; set; }
        public FinanceProviderProposalFlowType AncestorsCustomProposalFlow { get; set; }
        public Dictionary<string, ValueTypePair<SettingValueType, string>> ParentSettings { get; set; }
        //public bool HasTenantAncestor { get; set; }
    }
}
