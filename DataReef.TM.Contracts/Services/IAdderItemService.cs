using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataReef.TM.Models.Solar;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IAdderItemService : IDataService<AdderItem>
    {
        [OperationContract]
        int ApplyAdderUsageReduction(int usageInKwh, IEnumerable<Guid> adderItemIds);

        [OperationContract]
        decimal GetAdderCosts(IEnumerable<Guid> adderItemIds);
    }
}
