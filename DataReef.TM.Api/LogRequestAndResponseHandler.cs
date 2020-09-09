using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DataReef.TM.Api
{
    public class LogRequestAndResponseHandler : DelegatingHandler
    {
        private string requestBody = string.Empty;
        private string responseBody = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content != null)
            {
                requestBody = await request.Content.ReadAsStringAsync();

            }
            var result = await base.SendAsync(request, cancellationToken);
            if (result.Content != null)
            {
                responseBody = await result.Content.ReadAsStringAsync();
            }

            if (request.RequestUri.ToString().ToLowerInvariant().Contains(("/uploadDocument").ToLowerInvariant()))
            {
                ApiLogEntry apilog = new ApiLogEntry();
                apilog.Id = Guid.NewGuid();
                apilog.User = SmartPrincipal.UserId.ToString();
                apilog.Machine = Environment.MachineName;
                apilog.RequestContentType = "";
                apilog.RequestRouteTemplate = "";
                apilog.RequestRouteData = "";
                apilog.RequestIpAddress = "";
                apilog.RequestMethod = request.Method.Method;
                apilog.RequestHeaders = "";
                apilog.RequestTimestamp = DateTime.UtcNow;
                apilog.RequestUri = request.RequestUri.ToString();
                apilog.ResponseContentBody = responseBody;
                apilog.RequestContentBody = requestBody;

                using (var dc = new DataContext())
                {
                        dc.ApiLogEntries.Add(apilog);
                        dc.SaveChanges();
                }
            }
            return result;
        }
    }
}