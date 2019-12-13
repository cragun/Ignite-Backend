using DataReef.Core.Attributes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Integrations.Geo.DataViews;
using DataReef.Integrations.SolarCloud.DataViews;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using DataReef.Integrations.Common.Geo;

namespace DataReef.Integrations.SolarCloud
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ICloudBridge))]

    public class CloudBridge : ICloudBridge
    {
        private readonly string LegionAPIUrl = ConfigurationManager.AppSettings["DataReef.LegionAPI.Url"];
        private readonly string SolarCloudUrl = ConfigurationManager.AppSettings["DataReef.SolarCloud.Url"];

        private readonly Guid WorkflowLegionScoreId = new Guid("E4DB13B2-ABD0-40E3-8F71-8667BF50C526");
        private readonly Guid WorkflowClientId = new Guid("D1EBF792-20B9-4969-986A-B07C24112E46");

        private readonly IGeographyBridge GeoBridge;

        private RestClient Client
        {
            get
            {
                return new RestClient(SolarCloudUrl);
            }
        }

        public CloudBridge(IGeographyBridge geoBridge)
        {
            if (string.IsNullOrWhiteSpace(SolarCloudUrl))
            {
                throw new ApplicationException("No SolarCloud Url is configured.");
            }

            GeoBridge = geoBridge;
        }

        public JObject GetIngressWithResults(Guid ingressID)
        {
            string resource = string.Format("api/ingress/{0}/results", ingressID.ToString());
            var request = new RestRequest(resource, Method.GET);
            var response = Client.Execute(request);


            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    JObject ret = JObject.Parse(response.Content);
                    return ret;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return null;

        }


        public Guid InitiateWorkflowForNewLeads(WorkflowIngressRequest workflowRequest, bool addRoofAnalysis)
        {
            User workflowUser = new User();
            workflowUser.OUID = SmartPrincipal.OuId;
            workflowUser.UserID = SmartPrincipal.UserId;
            workflowUser.ClientID = WorkflowClientId;
            workflowRequest.User = workflowUser;
            //  TODO: move url to config
            workflowRequest.CompletionWebHookUrl = $"{LegionAPIUrl}/api/v1/webhooks/orderworkflowcompleted";

            if (addRoofAnalysis)
            {
                workflowRequest.WorkflowID = new Guid("02086C9C-721E-4AA8-912B-C0F65E4169D6"); //lead score with roof
            }
            else
            {
                workflowRequest.WorkflowID = new Guid("F182FB13-F766-473A-8B7A-E0E837E531AB"); //lead score
            }


            string resource = string.Format("api/ingress/workflows/{0}/leads", workflowRequest.WorkflowID.ToString());
            var request = new RestRequest(resource, Method.POST);

            string body = JsonConvert.SerializeObject(workflowRequest);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var response = Client.Execute(request);
            Guid g = Guid.Empty;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    WorkflowResponse wr = JsonConvert.DeserializeObject<WorkflowResponse>(response.Content);
                    g = wr.Guid;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return g;

        }

        public Guid InitiateWorkflowForWKT(string wkt, Guid territoryID, Guid prescreenBatchId)
        {

            //tags are an abstract property bag in the workflow.  For this particular workflow, we need to send in the territoryID that created the workflow
            if (territoryID == Guid.Empty)
            {
                throw new ApplicationException("Invalid TerritoryID");
            }

            List<Property> props = null;

            try
            {
                props = GeoBridge.GetPropertiesForWellKnownText(wkt).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }


            WorkflowIngressRequest workflowRequest = new DataViews.WorkflowIngressRequest();
            workflowRequest.TerritoryID = territoryID;
            workflowRequest.User = GetWorkflowUser(SmartPrincipal.OuId);
            workflowRequest.CompletionWebHookUrl = $"{LegionAPIUrl}/api/v1/webhooks/territoryworkflowcompleted/{prescreenBatchId}";
            workflowRequest.WorkflowID = WorkflowLegionScoreId;

            List<Lead> leads = new List<Lead>();

            foreach (var prop in props)
            {
                var lead = new Lead(prop);
                lead.TerritoryID = territoryID;
                leads.Add(lead);
            }

            workflowRequest.Leads = leads;
            workflowRequest.ExclusivityDays = 2;

            return MakeIngressRequest(workflowRequest);
        }

        public Guid InitiateWorkflowForLead(Lead lead, Guid ouid, Guid territoryId, Guid instantPrescreenId)
        {
            if (lead == null)
            {
                throw new ApplicationException("Invalid Lead!");
            }

            if (instantPrescreenId == Guid.Empty)
            {
                throw new ApplicationException("Invalid instant prescreen id!");
            }

            var workflowRequest = new WorkflowIngressRequest
            {
                TerritoryID = territoryId,
                User = GetWorkflowUser(ouid),
                CompletionWebHookUrl = $"{LegionAPIUrl}/api/v1/webhooks/propertyworkflowcompleted/{instantPrescreenId}",
                WorkflowID = WorkflowLegionScoreId
            };

            List<Lead> leads = new List<Lead> { lead };

            workflowRequest.Leads = leads;
            workflowRequest.ExclusivityDays = 2;

            return MakeIngressRequest(workflowRequest);
        }

        public Guid InitiateWorkflowForLeadEnhancement(ICollection<Lead> leads, bool addRoofAnalysis)
        {
            WorkflowIngressRequest workflowRequest = new WorkflowIngressRequest();
            workflowRequest.Leads = leads;
            workflowRequest.ExclusivityDays = 0;

            return InitiateWorkflowForNewLeads(workflowRequest, addRoofAnalysis);
        }

        private Guid MakeIngressRequest(WorkflowIngressRequest req)
        {
            string resource = $"api/ingress/workflows/{req.WorkflowID}/leads";
            var request = new RestRequest(resource, Method.POST);

            string body = JsonConvert.SerializeObject(req);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var response = Client.Execute(request);
            Guid g = Guid.Empty;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    WorkflowResponse wr = JsonConvert.DeserializeObject<WorkflowResponse>(response.Content);
                    g = wr.Guid;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return g;
        }

        private User GetWorkflowUser(Guid ouid)
        {
            return new User
            {
                OUID = ouid,
                UserID = SmartPrincipal.UserId,
                ClientID = WorkflowClientId
            };
        }

    }
}
