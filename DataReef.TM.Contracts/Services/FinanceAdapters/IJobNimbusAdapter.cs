using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services.FinanceAdapters
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IJobNimbusAdapter
    {
        [OperationContract]
        JobNimbusLeadResponseData CreateJobNimbusLead(Property property);
    }
}
