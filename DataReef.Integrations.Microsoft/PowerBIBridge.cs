using DataReef.Core.Attributes;
using DataReef.Core.Logging;
using DataReef.Integrations.Core;
using DataReef.Integrations.Microsoft.PowerBI.Models;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Microsoft
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IPowerBIBridge))]
    public class PowerBIBridge : BaseBridge, IPowerBIBridge
    {
        private ILogger _logger;
        public PowerBIBridge(ILogger logger)
        {
            _logger = logger;
            BaseUrl = ConfigurationManager.AppSettings["Microsoft.PowerBI.Url"];
            IsEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["Microsoft.PowerBI.IsEnabled"] ?? "false");
        }

        public void PushData(PBI_Base data)
        {
            if (data == null)
            {
                return;
            }
            PushDataAsync(new PBI_Base[] { data }).Wait();
        }

        public async Task PushDataAsync(PBI_Base data)
        {
            if (data == null)
            {
                return;
            }
            await PushDataAsync(new PBI_Base[] { data });
        }

        public void PushData(PBI_Base[] data)
        {
            PushDataAsync(data).Wait();
        }

        public async Task PushDataAsync(PBI_Base[] data)
        {
            try
            {
                if (data == null || data.Length == 0 || !IsEnabled)
                {
                    return;
                }

                var types = data.Select(d => d.GetType().Name).Distinct();

                foreach (var item in types)
                {
                    var path = PowerBIPathsProvider.GetPath(item);
                    var itemOfType = data.Where(d => d.GetType().Name == item).ToArray();

                    var req = BuildRequest(path, itemOfType);
                    await Client.ExecuteTaskAsync(req);
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
        }

        private RestRequest BuildRequest(string path, object data)
        {
            var req = new RestRequest(path, Method.POST);
            req.JsonSerializer = new RestSharpJsonSerializer();
            req.AddJsonBody(data);
            return req;
        }

    }
}
