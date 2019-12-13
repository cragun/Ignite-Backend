using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.EPC;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IEpcStatusService : IDataService<EpcStatus>
    {
        [OperationContract]
        void UploadEpcStatuses(Guid ouid, List<EpcStatusInput> epcStatuses);
    }
}
