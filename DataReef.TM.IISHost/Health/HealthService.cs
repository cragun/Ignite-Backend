using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using Microsoft.Practices.ServiceLocation;
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace DataReef.TM.IISHost.Health
{
    [ServiceContract]
    public interface IHealthService
    {
        [OperationContract]
        [AnonymousAccess]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string Check();


        [OperationContract]
        [AnonymousAccess]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        bool SimpleCheck();
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class HealthService : IHealthService
    {
        public string Check()
        {
            try
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                var appSettings = ServiceLocator.Current.GetInstance<IAppSettingService>();
                if (appSettings.IsHealthy())
                {
                    return "true";
                }
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return "Unhealthy!";
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return GetException(ex);
            }

            throw new ApplicationException("Unhealthy!");
        }

        public bool SimpleCheck()
        {
            return true;
        }

        private string GetException(Exception ex)
        {
            if (ex == null)
            {
                return null;
            }
            return $"{ex.Message} \r\nINNER: {GetException(ex.InnerException)}\r\n";
        }
    }
}
