using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace DataReef.TM.Api
{
    public class UserFriendlyExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
                ApiLogEntry apilog = new ApiLogEntry();
                apilog.Id = Guid.NewGuid();
                apilog.User = SmartPrincipal.UserId.ToString();
                apilog.Machine = Environment.MachineName;
                apilog.RequestContentType = "ErrorTest";
                apilog.RequestRouteTemplate = "";
                apilog.RequestRouteData = "";
                apilog.RequestIpAddress = context.Exception?.Message?.ToString();
                apilog.RequestMethod = context.Exception?.StackTrace?.ToString();
                apilog.RequestHeaders = context.Exception?.InnerException?.ToString();
                apilog.RequestTimestamp = DateTime.UtcNow;
                apilog.RequestUri = context?.ActionContext?.ActionArguments?.Values?.ToString();
                apilog.ResponseContentBody = context.Request?.ToString();
                apilog.RequestContentBody = context.Response?.StatusCode.ToString();

                using (var dc = new DataContext())
                {
                    dc.ApiLogEntries.Add(apilog);
                    dc.SaveChanges();
                }
        }
    }
}