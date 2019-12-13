using DataReef.Core.Attributes;
using NLog;
using DataReef.Core.Logging;

namespace DataReef.Core.Infrastructure.Logging
{
    [Service(typeof(DataReef.Core.Logging.ILogger))]
    public class SmartLogger : DataReef.Core.Logging.ILogger
    {
        private static Logger logger = LogManager.GetLogger("SmartLogger");

        public void Information(string message)
        {
            logger.Info(message);
        }

        public void Information(string message, params object[] args)
        {
            logger.Info(message, args);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            logger.Error(message, args);
        }

        public void Error(string message, System.Exception ex)
        {
            logger.Error(ex, message);
        }

        public void Warning(string message)
        {
            logger.Warn(message);
        }

        public void Warning(string message, params object[] args)
        {
            logger.Warn(message, args);
        }

        public void Trace(string message)
        {
            logger.Trace(message);
        }

        public void Trace(string message, params object[] args)
        {
            logger.Trace(message, args);
        }
    }
}
