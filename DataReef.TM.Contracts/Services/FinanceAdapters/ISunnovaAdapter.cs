using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services.FinanceAdapters
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ISunnovaAdapter
    {
        [OperationContract]
        List<SunnovaLead> CreateSunnovaLead(Property property);

        [OperationContract]
        SunnovaLeadCreditResponse PassSunnovaLeadCredit(Property property);

        [OperationContract]
        string GetSunnovaToken();
    }
}
