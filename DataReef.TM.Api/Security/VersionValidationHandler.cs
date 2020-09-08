using DataReef.Core;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Security
{
    public class VersionValidationHandler : DelegatingHandler
    {
        private readonly ILogger _logger;
        private readonly IAppSettingService _appSettingService = null;
        private static int CacheMinutes = 5;

        private static List<string> bypassValidationMidPoints = new List<string>
        {
            "hancock/proposals",
            "api/v1/webhooks",
            "api/v1/proposals/media/",
            "api/v1/proposals/data/",
            "api/v1/util",
            "api/v1/health",
            "api/v1/properties/attachments"
        };

        private static List<string> bypassValidationEndpoints = new List<string>
        {
            "health",
            "help",
            "sign/RightSignatureContractCallback",
            "sign/completed",
            "Solcius/Spruce/hardcreditcheck/callback",
            "Solcius/Spruce/documents/callback",
            "users/active",
        };

        /// <summary>
        /// Caching Minimum Version for 5 minutes.
        /// Otherwise every API request will hit the database first to get the Minimum Version
        /// </summary>
        /// <param name="appSettingService"></param>
        /// <returns></returns>
        private static Version GetVersionCache(IAppSettingService appSettingService)
        {
            Version version = MemoryCache.Default.Get("Version") as Version;
            if (version == null)
            {
                try
                {
                    version = appSettingService.GetMinimumRequiredVersionForIPad();
                }
                catch (Exception ex)
                {
                    return null;
                }
                MemoryCache.Default.Set("Version", version, new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.AddMinutes(CacheMinutes) });
            }
            return version;
        }



        public VersionValidationHandler(ILogger logger, IAppSettingService appSettingService)
        {
            _logger = logger;
            _appSettingService = appSettingService;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            foreach (var bypassValidationEndpoint in bypassValidationEndpoints)
            {
                if (request.RequestUri.AbsolutePath.ToLower().EndsWith(@"/" + bypassValidationEndpoint.ToLower())) return base.SendAsync(request, cancellationToken);
            }

            foreach (var midPoint in bypassValidationMidPoints)
            {
                if (request.RequestUri.AbsolutePath.ToLower().Contains(midPoint.ToLower())) return base.SendAsync(request, cancellationToken);
            }


            var minimumRequiredVersion = GetVersionCache(_appSettingService);
            if (minimumRequiredVersion == null)
            {
                return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Could not retrieve the minimum required version!") });
            }

            Version clientVersion;
            if (!TryGetClientVersion(request, out clientVersion)) return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.PreconditionFailed) { Content = new StringContent(String.Format("Could not get client version, the minimum required version is {0}.", minimumRequiredVersion)) });

            if (clientVersion < minimumRequiredVersion)
            {
                _logger.Information("The client version {0} is no longer supported", clientVersion);
                return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.PreconditionFailed) { Content = new StringContent(String.Format("This client version is no longer supported, the minimum required version is {0}.", minimumRequiredVersion)) });
            }

            return base.SendAsync(request, cancellationToken);
        }

        public bool TryGetClientVersion(HttpRequestMessage request, out Version clientVersion)
        {
            IEnumerable<string> headerValues;
            var clientVersionString = "";
            if (request.Headers.TryGetValues(RequestHeaders.ClientVersionHeaderName, out headerValues))
            {
                clientVersionString = headerValues.FirstOrDefault();
            }

            return Version.TryParse(clientVersionString, out clientVersion);
        }

    }
}