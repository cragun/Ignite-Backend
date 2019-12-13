using System;
using System.Diagnostics;

namespace DataReef.TM.DataAccess.Migrations.Seed
{
    //todo: temporary until we get solve the seeding issues
    internal static class SeedLogger
    {
        public static void Info(string message)
        {
            try
            {
                EventLog.WriteEntry("Application", message, EventLogEntryType.Information);
            }
            catch
            {
            }


        }

        public static void Error(string message)
        {
            try
            {
                EventLog.WriteEntry("Application", message, EventLogEntryType.Error);
            }
            catch
            {
            }
        }

        public static void Warning(string message)
        {
            try
            {
                EventLog.WriteEntry("Application", message, EventLogEntryType.Warning);
            }
            catch
            {
            }
        }

        public static void Error(Exception ex)
        {
            try
            {
                EventLog.WriteEntry("Application", ex.Message + "|" + ex.StackTrace, EventLogEntryType.Error);
            }
            catch
            {
            }
        }
    }
}