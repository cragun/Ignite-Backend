using System;
using System.Linq;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.DataAccess.Database;
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using System.ServiceModel.Activation;
using System.ServiceModel;
using DataReef.Core.Services;
using System.Collections.Generic;
using DataReef.TM.Models.Commerce;
using System.Linq.Dynamic;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Integrations.Mapful;
using DataReef.TM.Models.DTOs.Commerce;
using DataReef.Integrations.Mapful.DataViews;
using DataReef.TM.Models.Accounting;
using DataReef.Integrations.SolarCloud;
using DataReef.Integrations.SolarCloud.DataViews;
using DataReef.TM.Models.DTOs.Common;
using Newtonsoft.Json;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class OrderService : DataService<Order>, IOrderService
    {
        private readonly ITokensProvider _tokensProvider;
        private readonly IMapfulBridge _mapfulService;
        private readonly ICloudBridge _solarCloudService;

        public OrderService(ILogger logger,
                            ITokensProvider tokensProvider,
                            IMapfulBridge mapfulService,
                            ICloudBridge solarCloudService, Func<IUnitOfWork> unitOfWorkFactory
                            )
            : base(logger, unitOfWorkFactory)
        {
            _tokensProvider = tokensProvider;
            _mapfulService = mapfulService;
            _solarCloudService = solarCloudService;
        }

        public int CountPotentialLeads(CreateLeadsDto request)
        {
            var mapFulRequest = new AnalyzeRequest
            {
                Filters = request
                            .GetMapfulFriendly()
                            .Select(r => new Filter
                            {
                                FieldID = r.Key,
                                Value1 = r.Value?.FirstOrDefault(),
                                Value2 = r.Value?.Length > 1 ? r.Value.LastOrDefault() : null
                            }).ToList()
            };

            return _mapfulService.CountProperties(mapFulRequest);
        }

        public void CreateOrderForNewLeads(CreateLeadsDto request)
        {
            #region validation

            //get the guid of the person logged in ( provided by our Auth framework)
            Guid userID = SmartPrincipal.UserId;

            //check to see if they have any balance first
            TokenLedger ledger = _tokensProvider.GetDefaultLedgerForPerson(userID);

            //if ledger == null then the user has no credits available, no ledger=no credits
            if (ledger == null)
            {
                throw new ApplicationException("Insufficient Credits Available.  No Ledger Account Exists.  Contact Your DataReef Representative.");
            }

            if (request.MaxNumberOfLeads == 0)
            {
                throw new ApplicationException("Missing Parameter. Maximum Number Of Leads");
            }

            if (request.LengthOfExclusivity == 0) request.LengthOfExclusivity = 30;

            int exclusiveMonths = request.LengthOfExclusivity / 30;
            int tokensRequired = request.MaxNumberOfLeads * exclusiveMonths; ;

            double balance = _tokensProvider.GetBalanceForLedger(ledger.Guid);
            if (balance < tokensRequired)
            {
                throw new ApplicationException("Insufficient Credits Available");
            }

            #endregion

            #region create solar leads

            var leadsRequest = new LeadsRequest
            {
                RecordCount = request.MaxNumberOfLeads,
                Filters = request
                           .GetMapfulFriendly()
                           .Select(r => new Filter
                           {
                               FieldID = r.Key,
                               Value1 = r.Value?.FirstOrDefault(),
                               Value2 = r.Value?.Length > 1 ? r.Value.LastOrDefault() : null
                           }).ToList()
            };

            //workflow will get its own leads based on the FilterJson
            //var mapfulLeads = _mapfulService.GetLeads(leadsRequest);


            WorkflowIngressRequest workflowRequest = new WorkflowIngressRequest();
            workflowRequest.ExclusivityDays = request.LengthOfExclusivity;
            workflowRequest.FilterJson = JsonConvert.SerializeObject(leadsRequest);
            workflowRequest.TargetRecordCount = request.MaxNumberOfLeads;

            Guid batchID = _solarCloudService.InitiateWorkflowForNewLeads(workflowRequest,false);

            #endregion

            #region create order and token expense

            using (DataContext dc = new DataContext())
            {
                Order order = new Order();
                order.Guid = batchID; //reuse the batchID so its easy to find and link the order and the workflow
                order.OrderType = OrderType.NewLeads;
                order.PersonID = userID;
                order.CreatedByID = userID;
                order.WorkflowId = new Guid("F182FB13-F766-473A-8B7A-E0E837E531AB"); //lead gen.  todo: make this data driven
                order.Status = OrderStatus.InProgress;
                order.InitialRecords = request.MaxNumberOfLeads;
                dc.Orders.Add(order);

                //first create put the token pre-auth.  This will be replaced with the actual number after the workflow returns

                //now create the accounting 
                //first create an expense for ALL the houses available. Leter we credit back the unused portionn
                var expense = new TokenExpense
                {
                    Amount = tokensRequired,
                    CreatedByID = userID,
                    DateCreated = System.DateTime.UtcNow,
                    LedgerID = ledger.Guid,
                    TenantID = SmartPrincipal.TenantId,
                    ExternalID = order.Guid.ToString(),
                    Notes = "Order Preauthorization",
                    TagString = "preauth"

                };
                dc.TokenExpenses.Add(expense);

                //save the changes to db.. 
                dc.SaveChanges();

                //next lets get the actual records to kick off the workflow
                //todo: after SolarWorld demo, workflow should get its own records based on the request.

            }

            #endregion


        }

        public PaginatedResult<Order> GetOrders(Guid userId, int pageIndex = 0, int pageSize = 20, string sortColumn = "DateCreated", int sortOrder = 1)
        {
            using (var dc = new DataContext())
            {
                var result = new PaginatedResult<Order>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                var order = sortOrder == 1 ? "" : "DESC";
                var query = dc
                            .Orders
                            .Where(o => o.CreatedByID == userId);
                result.Total = query.Count();
                result.Data = query
                                .OrderBy($"{sortColumn} {order}")
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .ToList();
                return result;
            }
        }

        public double GetTokensBalance(Guid userId)
        {
            var ledger = _tokensProvider.GetDefaultLedgerForPerson(userId);
            if (ledger == null)
                return 0;

            var balance = _tokensProvider.GetBalanceForLedger(ledger.Guid);
            return balance;
        }

        public PaginatedResult<OrderDetail> GetOrderDetails(Guid OrderId, Guid userId, int pageIndex = 0, int pageSize = 20, string sortColumn = "DateCreated", int sortOrder = 1)
        {
            using (var dc = new DataContext())
            {
                var result = new PaginatedResult<OrderDetail>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                var order = sortOrder == 1 ? "" : "DESC";
                var query = dc
                            .OrderDetails
                            .Where(o => o.CreatedByID == userId && o.OrderID == OrderId);
                result.Total = query.Count();
                result.Data = query
                                .OrderBy($"{sortColumn} {order}")
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .ToList();
                return result;
            }
        }


        public void CreateOrderForLeadEnhancement(CreateLeadEnhancementDto request, bool addRoofAnalysis)
        {

            #region validation

            //get the guid of the person logged in ( provided by our Auth framework)
            Guid userID = SmartPrincipal.UserId;

            //check to see if they have any balance first
            TokenLedger ledger = _tokensProvider.GetDefaultLedgerForPerson(userID);

            //if ledger == null then the user has no credits available, no ledger=no credits
            if (ledger == null)
            {
                throw new ApplicationException("Insufficient Credits Available.  No Ledger Account Exists.  Contact Your DataReef Representative.");
            }

            if (request.UploadedLeads.Count == 0)
            {
                throw new ApplicationException("Nothing to do.  No Leads were uploaded");
            }


            int tokensRequired = request.UploadedLeads.Count;

            double balance = _tokensProvider.GetBalanceForLedger(ledger.Guid);
            if (balance < tokensRequired)
            {
                throw new ApplicationException("Insufficient Credits Available");
            }

            #endregion

            #region create solar leads


            Guid workflowID = addRoofAnalysis ? new Guid("02086C9C-721E-4AA8-912B-C0F65E4169D6") : new Guid("F182FB13-F766-473A-8B7A-E0E837E531AB");



            List<Lead> solarLeads = new List<Lead>();
            foreach (var ul in request.UploadedLeads)
            {
                Lead l = new Lead();
                l.ExternalID = ul.Identifier;
                l.Address = ul.Address;
                l.City = ul.City;
                l.State = ul.State;
                l.ZipCode = ul.ZipCode;
                l.FirstName = ul.FirstName;
                l.LastName = ul.LastName;
                l.Phone = ul.Phone;
                l.JanConsumption = ul.JanuaryConsumption;
                l.FebConsumption = ul.FebruaryConsumption;
                l.MarConsumption = ul.MarchConsumption;
                l.AprConsumption = ul.AprilConsumption;
                l.MayConsumption = ul.MayConsumption;
                l.JunConsumption = ul.JuneConsumption;
                l.JulConsumption = ul.JulyConsumption;
                l.AugConsumption = ul.AugustConsumption;
                l.SepConsumption = ul.SeptemberConsumption;
                l.OctConsumption = ul.OctoberConsumption;
                l.NovConsumption = ul.NovemberConsumption;
                l.DecConsumption = ul.DecemberConsumption;
                solarLeads.Add(l);

            }

            Guid batchID = _solarCloudService.InitiateWorkflowForLeadEnhancement(solarLeads, addRoofAnalysis);

            #endregion

            #region create order and token expense

            using (DataContext dc = new DataContext())
            {
                Order order = new Order();
                order.Guid = batchID; //reuse the batchID so its easy to find and link the order and the workflow
                order.OrderType = OrderType.NewLeads;
                order.PersonID = userID;
                order.CreatedByID = userID;
                order.WorkflowId = workflowID; //lead gen.  todo: make this data driven
                order.Status = OrderStatus.InProgress;
                order.InitialRecords = request.UploadedLeads.Count; ;
                dc.Orders.Add(order);

                //first create put the token pre-auth.  This will be replaced with the actual number after the workflow returns

                //now create the accounting 
                //first create an expense for ALL the houses available. Leter we credit back the unused portionn
                var expense = new TokenExpense
                {
                    Amount = tokensRequired,
                    CreatedByID = userID,
                    DateCreated = System.DateTime.UtcNow,
                    LedgerID = ledger.Guid,
                    TenantID = SmartPrincipal.TenantId,
                    ExternalID = order.Guid.ToString(),
                    Notes = "Order Preauthorization",
                    TagString = "preauth"

                };
                dc.TokenExpenses.Add(expense);

                //save the changes to db.. 
                dc.SaveChanges();

                //next lets get the actual records to kick off the workflow
                //todo: after SolarWorld demo, workflow should get its own records based on the request.

            }

            #endregion

        }

    }
}
