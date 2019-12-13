using System;
using System.ServiceModel;
using DataReef.TM.Models.Credit;
using DataReef.TM.Models.Enums;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPrescreenBatchService : IDataService<PrescreenBatch>
    {
        [OperationContract]
        void UpdateStatus(string externalId, PrescreenStatus newStatus);

        [OperationContract]
        void UpdateStatusById(Guid prescreenBatchId, PrescreenStatus newStatus, int processedHousesCount);
    }
}