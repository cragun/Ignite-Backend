using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DataReef.Core.Infrastructure.Authorization
{
    /// <summary>
    /// This attribute is used to populate CurrentPrincipal based on the data in .config file.
    /// We created this method to remove the AnonymousAccess attributes on Core methods
    /// </summary>
    public class InjectAuthPrincipalAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            var userId = ConfigurationManager.AppSettings["AnonymousPrincipal_UserID"];
            var ouId = ConfigurationManager.AppSettings["AnonymousPrincipal_OUID"];
            var accountId = ConfigurationManager.AppSettings["AnonymousPrincipal_AccountID"];
            var clientVersion = ConfigurationManager.AppSettings["AnonymousPrincipal_ClientVersion"];

            var emptyGuid = Guid.Empty.ToString();

            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(DataReefClaimTypes.UserId, userId),
                    new Claim(DataReefClaimTypes.DeviceId, emptyGuid),
                    new Claim(DataReefClaimTypes.OuId, ouId),
                    new Claim(DataReefClaimTypes.TenantId, "1"),
                    new Claim(DataReefClaimTypes.AccountID, accountId),
                    new Claim(DataReefClaimTypes.ClientVersion, clientVersion),
                    new Claim(DataReefClaimTypes.DeviceDate, DateTime.UtcNow.ToString())
                }, "InternalBearer")); // is authenticated
        }
    }
}
