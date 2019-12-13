using System;

namespace DataReef.Core.Logging
{
    /// <summary>
    /// Infrastructure interface for logging.
    /// </summary>
    public interface ILogger
    {
        void Trace(string message);

        void Trace(string message, params object[] args);

        void Information(string message);

        void Information(string message, params object[] args);

        void Error(string message);

        void Error(string message, params object[] args);

        void Error(string message, Exception ex);

        void Warning(string message, params object[] args);
    }
}

