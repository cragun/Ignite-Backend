using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Core.Attributes;
using DataReef.Core.Common;
using DataReef.Integrations.RedBell.RedBellBeta;

namespace DataReef.Integrations.RedBell
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IRedBellAdapter))]
    public class RedBellAdapter : IRedBellAdapter
    {
        private readonly string _redBellUsername = ConfigurationManager.AppSettings[RedBellConfigurationKeys.Username];
        private readonly string _redBellLivePassword = ConfigurationManager.AppSettings[RedBellConfigurationKeys.Password];
        //private const string RedBellBetaPassword = "betaDR02817!";

        public decimal GetAveValue(OrderRequest rbRequest)
        {
            try
            {
                var client = GetOrderClient();
                var rbResponse = client.UploadSync(rbRequest);

                var ave = rbResponse.ReturnData.Deserialize<Ave>();

                return ave.AveValue;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error connecting with red bell", ex);
            }
        }

        private OrderServiceClient GetOrderClient()
        {
            var client = new OrderServiceClient();

            client.ClientCredentials.UserName.UserName = _redBellUsername;
            client.ClientCredentials.UserName.Password = _redBellLivePassword;

            return client;
        }
    }
}
