using System.ServiceModel;
using DataReef.TM.Models;
using System.Collections.Generic;
using System;
using DataReef.TM.Models.Commerce;
using DataReef.TM.Models.DTOs.Commerce;
using DataReef.TM.Models.DTOs.Common;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IOrderService : IDataService<Order>
    {
        [OperationContract]
        PaginatedResult<Order> GetOrders(Guid userId, int pageIndex = 0, int pageSize = 20, string sortColumn = "DateCreated", int sortOrder = 1);

        [OperationContract]
        PaginatedResult<OrderDetail> GetOrderDetails(Guid OrderId, Guid userId, int pageIndex = 0, int pageSize = 20, string sortColumn = "DateCreated", int sortOrder = 1);

        [OperationContract]
        double GetTokensBalance(Guid userId);

        [OperationContract]
        int CountPotentialLeads(CreateLeadsDto request);

        [OperationContract]
        void CreateOrderForNewLeads(CreateLeadsDto request);

        [OperationContract]
        void CreateOrderForLeadEnhancement(CreateLeadEnhancementDto request, bool addRoofAnalysis);
    }
}