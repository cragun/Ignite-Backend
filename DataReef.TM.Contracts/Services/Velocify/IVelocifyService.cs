using System.ServiceModel;
using DataReef.TM.Models.PRMI;
using System;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IVelocifyService
    {
        [OperationContract]
        VelocifyResponse SendProposal(Guid? ouID, VelocifyRequest request);
    }
}
