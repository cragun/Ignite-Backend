using DataReef.TM.Models.Credit;
using DataReef.TM.Models.Enums;
using System;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPrescreenInstantService : IDataService<PrescreenInstant>
    {
        [OperationContract]
        void UpdateStatusById(Guid prescreenInstantId, PrescreenStatus newStatus, int processedHousesCount);
    }
}