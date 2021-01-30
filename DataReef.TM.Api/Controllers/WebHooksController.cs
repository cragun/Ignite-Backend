using DataReef.Core.Infrastructure.Authorization;
using DataReef.Integrations.SolarCloud;
using DataReef.TM.Api.Controllers.SignalR;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.Commerce;
using DataReef.TM.Models.Credit;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{

    [AllowAnonymous]
    [RoutePrefix("api/v1/webhooks")]
    public class WebHooksController : ApiController
    {
        private readonly IOrderService _orderService;
        private readonly IPrescreenBatchService _prescreenService;
        private readonly ICloudBridge _solarCloudService;
        private readonly IDataService<OrderDetail> _orderDetailService;
        private readonly Func<IPropertyService> _propertyServiceFactory;
        private Lazy<IPrescreenInstantService> _prescreenInstantService;

        public WebHooksController(
            IPrescreenBatchService prescreenService,
            IOrderService orderService,
            ICloudBridge solarCloudService,
            IDataService<OrderDetail> orderDetailService,
            Func<IPropertyService> propertyServiceFactory,
            Lazy<IPrescreenInstantService> prescreenInstantService)
        {
            _prescreenService = prescreenService;
            _orderService = orderService;
            _solarCloudService = solarCloudService;
            _orderDetailService = orderDetailService;
            _propertyServiceFactory = propertyServiceFactory;
            _prescreenInstantService = prescreenInstantService;
        }

        [AllowAnonymous]
        [InjectAuthPrincipal]
        [Route("territoryworkflowcompleted/{prescreenBatchId}")]
        [HttpPost]
        public async Task<IHttpActionResult> TerritoryWorkflowCompleted([FromBody] dynamic body, Guid prescreenBatchId)
        {
            string json = Convert.ToString(body);
            var jo = JObject.Parse(json);
            var objectCount = jo["ObjectCount"];

            int processedHousesCount = 0;
            if (objectCount != null) int.TryParse(objectCount.ToString(), out processedHousesCount);

            _prescreenService.UpdateStatusById(prescreenBatchId, Models.Enums.PrescreenStatus.Completed, processedHousesCount);

            //  sync property attributes from Geo with TM
            try
            {
              await Task.Factory.StartNew(() => _propertyServiceFactory().SyncPrescreenBatchPropertiesAttributes(prescreenBatchId));
            }
            catch (Exception)
            {
            }

            return Ok();
        }

        [AllowAnonymous]
        [InjectAuthPrincipal]
        [Route("propertyworkflowcompleted/{prescreenInstantId}")]
        [HttpPost]
        public async Task<IHttpActionResult> PropertyWorkflowCompleted([FromBody]dynamic body, [FromUri]Guid prescreenInstantId)
        {
            string json = Convert.ToString(body);
            var jo = JObject.Parse(json);
            var objectCount = jo["ObjectCount"];

            int processedHousesCount = 0;
            if (objectCount != null) int.TryParse(objectCount.ToString(), out processedHousesCount);

            _prescreenInstantService.Value.UpdateStatusById(prescreenInstantId, Models.Enums.PrescreenStatus.Completed, processedHousesCount);

            //  sync property attributes from Geo with TM
            try
            {
              await  Task.Factory.StartNew(() =>
                {
                    _propertyServiceFactory().SyncInstantPrescreenPropertyAttributes(prescreenInstantId);
                    // TODO: send a push notification once the service is in place
                });
            }
            catch (Exception)
            {
            }

            return Ok();
        }

        [AllowAnonymous]
        [InjectAuthPrincipal]
        [Route("orderworkflowcompleted")]
        [HttpPost]
        public async Task<IHttpActionResult> OrderWorkflowCompleted([FromBody] dynamic body)
        {

         

                string json = Convert.ToString(body);
                JObject jo = JObject.Parse(json);

                Guid g = new Guid(jo["Guid"].ToString());

                //first thing we need to do is get the order and update it

                Order order = await _orderService.Get(g, "Details");

                if (order != null)
                {
                    //first get all the finished data from workflow

                    JObject ingress = _solarCloudService.GetIngressWithResults(g);
                    if (ingress == null || ingress["Results"] == null || ingress["Results"].Count() == 0) return null;

                    Guid userID = new Guid(ingress["Results"][0]["User"]["UserID"].ToString());
                    SmartPrincipal.ImpersonateUser(userID, Guid.NewGuid());

                    if (ingress != null && ingress["Results"] != null)
                    {
                        List<OrderDetail> orderDetails = new List<OrderDetail>();
                        var results = ingress["Results"];

                        foreach (var result in results)
                        {
                            string packageID = result["PackageNumber"].ToString();

                            if (order.Details == null || order.Details.FirstOrDefault(odd => odd.ExternalID.ToLower() == packageID.ToLower()) == null)
                            {
                                OrderDetail od = new OrderDetail();
                                od.Json = result.ToString();
                                od.OrderID = order.Guid;
                                od.ExternalID = packageID;
                                orderDetails.Add(od);
                            }
                        }

                        if (orderDetails.Any()) _orderDetailService.InsertMany(orderDetails);
                    }


                    //last lets update the order status
                    order.Status = OrderStatus.Completed;
                    _orderService.Update(order);
                    NotifyWebClients(order);
                }
        

            return Ok();

        }

        private void NotifyWebClients(Order order)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<PortalHub>();
            if (hubContext != null)
            {
                hubContext
                    .Clients
                    .Group(order.PersonID.ToString())
                    .orderStatusChanged(new
                    {
                        OrderGuid = order.Guid,
                        OrderId = order.Id,
                        Status = order.Status
                    });
            }
        }
    }
}
