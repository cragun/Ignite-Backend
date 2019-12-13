using System;
using System.ServiceModel;
using DataReef.TM.Models.DTOs.Mortgage;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IMortgageService
    {
        [OperationContract]
        MortgageSource GetMortgageDetails(Guid propertyId);

        [OperationContract]
        MortgageResponse GetMortgageDetailsFiltered(MortgageRequest request);
    }
}
