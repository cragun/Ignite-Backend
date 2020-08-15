using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Security
{
    internal class CorsValidationHandler : DelegatingHandler
    {
        private const string OriginRequestHeader = "Origin";
        private const string AccessControlAlloHeadersKey = "Access-Control-Allow-Headers";
        private const string AccessControlAllowOriginKey = "Access-Control-Allow-Origin";
        private const string AccessControlAllowMethodsKey = "Access-Control-Allow-Methods";
        private const string AllowedRequestOriginKey = "AllowedRequestOrigins";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(OriginRequestHeader))
                return await base.SendAsync(request, cancellationToken);

            string requestOrigin = request.Headers.GetValues(OriginRequestHeader).First();
            string domain = string.Empty;
            try
            {
                var uri = new Uri(requestOrigin);
                domain = uri.Host;
            }
            catch { }

            IEnumerable<string> allowedOrigins = (ConfigurationManager.AppSettings[AllowedRequestOriginKey] ?? "").Split(';');

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            if (string.IsNullOrWhiteSpace(domain) || !allowedOrigins.Any(o => domain.StartsWith(o, StringComparison.CurrentCultureIgnoreCase)))
            {
                return await ((request.Method == HttpMethod.Options) ? Task.FromResult(response) : base.SendAsync(request, cancellationToken));
            }

            // request the browser to enter auth, consider removing it as not all browsers can take the full length of the token inside the user box
            response = await ((request.Method == HttpMethod.Options) ? Task.FromResult(response) : base.SendAsync(request, cancellationToken));
            response.Headers.Add(AccessControlAlloHeadersKey, "Authorization, DataReef-FullVersion, Content-Type, Cache-Control, X-Requested-With");
            response.Headers.Add(AccessControlAllowOriginKey, requestOrigin);
            response.Headers.Add(AccessControlAllowMethodsKey, "PATCH, POST, GET, OPTIONS, DELETE");

            return response;
        }
    }
}