using System.ServiceModel;
using DataReef.TM.Models;
using System.Collections.Generic;
using System;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IMediaItemService : IDataService<MediaItem>
    {
        [OperationContract]
        ICollection<MediaItem> GetMediaItems(Guid ouID, bool deletedItems = false, int type = -1);
    }
}