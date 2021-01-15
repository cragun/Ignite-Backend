using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace DataReef.TM.Api.Logging
{
    public class CustomLogger : ExceptionLogger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public override void Log(ExceptionLoggerContext context)
        {
            if (log != null && context != null && context.Exception != null)
            {
                log.Error(context.Exception.Message, context.Exception);
            }
        }
    }
}