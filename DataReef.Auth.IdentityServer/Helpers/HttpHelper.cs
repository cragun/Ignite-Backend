using Microsoft.Owin;
using Newtonsoft.Json;
using DataReef.Auth.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace DataReef.Auth.IdentityServer.Helpers
{
    class HttpHelper
    {
        internal static void StashResponseDetails(HttpStatusCode statusCode, string message = null, object data = null)
        {
            AuthenticationResponse responseDetails = new AuthenticationResponse()
            {
                StatusCode = statusCode,
                Message = message,
                Data = data
            };

            if (HttpContext.Current == null) return;
            IOwinContext context = HttpContext.Current.GetOwinContext();
            if (context == null) return;

            // Add detailed information about the authentication failure reason in the OWIN environment dictionary
            // A later processor in the OWIN pipeline will pick it up from there and setup the HTTP response message accordingly
            context.Set(AuthConstants.AuthenticationReponseKey, responseDetails);
        }
    }
}