using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Infrastructure.Logging
{
    public class LoggerFactory
    {
        public static Serilog.Core.Logger Create()
        {
            //use the same db setting used in stored in the config
            var logDB = System.Configuration.ConfigurationManager.ConnectionStrings[Settings.LoggingConnectionStringName].ConnectionString;
            var logTable = "Logs";

            var options = new ColumnOptions();
            options.Store.Remove(StandardColumn.Properties);
            options.Store.Add(StandardColumn.LogEvent);

            Serilog.Debugging.SelfLog.Enable(Console.Out);

            return new LoggerConfiguration()
                .WriteTo.MSSqlServer(
                    connectionString: logDB,
                    tableName: logTable,
                    columnOptions: options,
                    autoCreateSqlTable:true
                ).CreateLogger();
        }
    }
}
