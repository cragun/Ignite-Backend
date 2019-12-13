using DataReef.Integrations.Mapful.DataViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.ApplicationServices;

namespace DataReef.Integrations.Mapful
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IMapfulBridge
    {
        [OperationContract]
        int CountProperties(AnalyzeRequest request);

        [OperationContract]
        ICollection<LeadDataView> GetLeads(LeadsRequest request);


    }
}
