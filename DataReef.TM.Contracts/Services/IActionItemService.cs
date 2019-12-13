using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.EPC;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IActionItemService : IDataService<PropertyActionItem>
    {
        [OperationContract]
        void UploadActionItems(Guid ouid, List<ActionItemInput> actionItems);
    }
}