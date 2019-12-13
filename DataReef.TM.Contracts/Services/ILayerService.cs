using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataReef.TM.Models;
using DataReef.TM.Models.Layers;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ILayerService : IDataService<Layer>
    {
        [OperationContract]
        ICollection<Layer> GetLayersForOU(Guid ouID, bool deletedItems);
        
    }
}