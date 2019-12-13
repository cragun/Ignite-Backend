using DataReef.Core;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace DataReef.TM.Api.Security
{
    internal class CustomAuthorizationFilter : AuthorizeAttribute
    {
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Basic";
        private const string UnauthorizedReasonKey = "UnauthorizedReason";

        private readonly IAuthenticationService _authenticationService = null;
        public CustomAuthorizationFilter(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (SkipAuthorization(actionContext))
                return true;

            bool isUserAuthenticated = SmartPrincipal.IsAuthenticated && SmartPrincipal.IsValid;
            if (!isUserAuthenticated)
            {
                actionContext.ActionArguments.Add(UnauthorizedReasonKey, "Authentication failed!");
                return false;
            }

            bool isUserActive = IsUserActive(Thread.CurrentPrincipal);
            if (!isUserActive)
            {
                actionContext.ActionArguments.Add(UnauthorizedReasonKey, "Account is suspended!");
                return false;
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
            var response = actionContext.Response;
            if (actionContext.ActionArguments.ContainsKey(UnauthorizedReasonKey)) response.Content = new StringContent(actionContext.ActionArguments[UnauthorizedReasonKey].ToString());
            response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
        }
        private bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext?.ActionDescriptor?.GetCustomAttributes<AllowAnonymousAttribute>()?.Any() == true
                   || actionContext?.ControllerContext?.ControllerDescriptor?.GetCustomAttributes<AllowAnonymousAttribute>()?.Any() == true;
        }

        private bool IsUserActive(IPrincipal currentPrincipal)
        {
            var principal = currentPrincipal as ClaimsPrincipal;
            if (principal == null) return false;

            var userIdClaim = principal.FindFirst(DataReefClaimTypes.UserId);
            if (userIdClaim == null) return false;

            Guid userId = Guid.Empty;
            if (Guid.TryParse(userIdClaim.Value, out userId))
            {
                return _authenticationService.IsUserActive(userId);
            }

            return false;
        }
    }
}