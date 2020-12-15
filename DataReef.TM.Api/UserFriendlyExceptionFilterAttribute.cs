using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
//using Serilog;
using System;
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
            apilog.RequestContentType = "ErrorException";
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

            #region Send Exception Log File to datadog agent.

            var path = HttpContext.Current.Request.MapPath("/Datadog/logs/log.json");

            //var log = new Serilog.LoggerConfiguration()
            //    .Enrich.FromLogContext()
            //    .WriteTo.File(new Serilog.Formatting.Json.JsonFormatter(renderMessage: true), path)
            //    .CreateLogger();

            var request = new
            {
                User = SmartPrincipal.UserId.ToString(),
                Exception = context.Exception?.Message?.ToString(),
                Request = context.Request?.ToString(),
                RequestContentBody = context.Response?.StatusCode.ToString()
            }; 

           // log.Information("Exception - {@request}", request);

            #endregion


        }
    }
}