using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.OnBoarding
{
    [DataContract]
    [NotMapped]
    public class FinancePlanDataView : GuidNamePair
    {
        [DataMember]
        public FinanceProviderProposalFlowType FlowType { get; set; }

        [DataMember]
        public FinancePlanIntegrationProvider IntegrationProvider { get; set; }

        [DataMember]
        public double? DefaultDealerFee { get; set; }

        [DataMember]
        public string MetaData { get; set; }

        public FinancePlanDataView() : base()
        { }
    }
}
