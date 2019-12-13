using DataReef.Core;
using DataReef.Core.Enums;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Security
{
    /// <summary>
    /// Extracts the information from the request headers and adds them to the current user claims
    /// claims are set by the Authorization token in the tokenValidationHandler... and here from headers (in addition to token)
    /// </summary>
    internal class ClaimsTransformationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            if (principal == null)
                return base.SendAsync(request, cancellationToken);

            var identity = principal.Identity as ClaimsIdentity;
            if (identity == null)
                return base.SendAsync(request, cancellationToken);

            var clientVersion = GetHeaderValue(request, RequestHeaders.ClientVersionHeaderName);
            if (string.IsNullOrWhiteSpace(clientVersion))
            {
                return base.SendAsync(request, cancellationToken);
            }
            else
            {
                identity.AddClaim(new Claim(DataReefClaimTypes.ClientVersion, clientVersion));
            }

            var deviceId = GetHeaderValue(request, RequestHeaders.DeviceIDHeaderName);
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                identity.AddClaim(new Claim(DataReefClaimTypes.DeviceId, Guid.Empty.ToString()));
            }
            else
            {
                identity.AddClaim(new Claim(DataReefClaimTypes.DeviceId, deviceId));
            }

            var deviceDate = GetHeaderValue(request, RequestHeaders.DeviceDateHeaderName);
            if (string.IsNullOrWhiteSpace(deviceDate))
            {
                identity.AddClaim(new Claim(DataReefClaimTypes.DeviceDate, ((DateTimeOffset)DateTime.UtcNow).ToString()));
            }
            else
            {
                identity.AddClaim(new Claim(DataReefClaimTypes.DeviceDate, deviceDate));
            }

            var userAgent = GetHeaderValue(request, "User-Agent", false);
            var deviceType = userAgent.GetDeviceType().ToString();

            identity.AddClaim(new Claim(DataReefClaimTypes.DeviceType, deviceType));

            if (principal.Identity.IsAuthenticated == false) return base.SendAsync(request, cancellationToken);

            //TODO: temporary code - devices should send this info in request headers
            if (request.Headers.Contains(RequestHeaders.OUIDHeaderName) == false)
                request.Headers.Add(RequestHeaders.OUIDHeaderName, "49959FF8-5D02-4CA6-A222-B796DCC18EBC");

            if (request.Headers.Contains(RequestHeaders.AccountIDHeaderName) == false)
                request.Headers.Add(RequestHeaders.AccountIDHeaderName, "4185317C-6DD9-4591-8561-162CC09921EC");

            if (!request.Headers.Contains(RequestHeaders.DeviceTypeHeaderName))
            {
                request.Headers.Add(RequestHeaders.DeviceTypeHeaderName, deviceType);
            }

            var subjectClaim = principal.Claims.FirstOrDefault(c => c.Type == DataReefClaimTypes.UserId);
            if (subjectClaim == null || string.IsNullOrWhiteSpace(subjectClaim.Value))
                return base.SendAsync(request, cancellationToken);

            var tenantClaim = principal.Claims.FirstOrDefault(c => c.Type == DataReefClaimTypes.TenantId);
            if (tenantClaim == null || string.IsNullOrWhiteSpace(tenantClaim.Value))
                return base.SendAsync(request, cancellationToken);

            var ouId = GetHeaderValue(request, RequestHeaders.OUIDHeaderName);
            if (string.IsNullOrWhiteSpace(ouId))
                return base.SendAsync(request, cancellationToken);

            var accountID = GetHeaderValue(request, RequestHeaders.AccountIDHeaderName);
            if (string.IsNullOrWhiteSpace(accountID))
                return base.SendAsync(request, cancellationToken);

            identity.AddClaim(new Claim(DataReefClaimTypes.TenantId, tenantClaim.Value));
            identity.AddClaim(new Claim(DataReefClaimTypes.OuId, ouId));
            identity.AddClaim(new Claim(DataReefClaimTypes.AccountID, accountID));

            return base.SendAsync(request, cancellationToken);
        }

        private static string GetHeaderValue(HttpRequestMessage request, string headerName, bool first = true)
        {
            if (!request.Headers.Contains(headerName))
                return string.Empty;

            var values = request.Headers.GetValues(headerName);
            return first ? values.FirstOrDefault() : string.Join(" ", values);
        }
    }
}