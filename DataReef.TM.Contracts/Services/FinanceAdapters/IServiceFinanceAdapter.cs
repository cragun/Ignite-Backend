using System.ServiceModel;
using DataReef.TM.Models.DTOs.FinanceAdapters;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IServiceFinanceAdapter
    {
        [OperationContract]
        SubmitApplicationResponse SubmitApplication(SubmitApplicationRequest request);
    }
}
