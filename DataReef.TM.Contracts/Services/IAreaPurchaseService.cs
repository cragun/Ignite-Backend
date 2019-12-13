using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.ZipCodes;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    public interface IAreaPurchaseService: IDataService<AreaPurchase>
    {
        [OperationContract]
        ICollection<AreaPurchase> GetPurchasesForOU(Guid ouid);

        [OperationContract]
        ICollection<AreaPurchaseDto> GetAllPendingPurchases();

        [OperationContract]
        void UpdateProcessedPurchases(ICollection<AreaPurchase> processedPurchases);
    }
}
