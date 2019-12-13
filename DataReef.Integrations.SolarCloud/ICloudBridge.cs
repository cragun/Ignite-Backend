using DataReef.Integrations.Common.Geo;
using DataReef.Integrations.SolarCloud.DataViews;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.ApplicationServices;

namespace DataReef.Integrations.SolarCloud
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ICloudBridge
    {
        Guid InitiateWorkflowForNewLeads(WorkflowIngressRequest workflowRequest, bool addRoofAnalysis);

        Guid InitiateWorkflowForWKT(string wkt, Guid territoryID, Guid prescreenBatchId);
        Guid InitiateWorkflowForLead(Lead lead, Guid ouid, Guid territoryId, Guid instantPrescreenId);

        Guid InitiateWorkflowForLeadEnhancement(ICollection<Lead> leads, bool addRoofAnalysis);

        JObject GetIngressWithResults(Guid ingressID);

    }
}