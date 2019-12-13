namespace DataReef.Core
{
    public class Settings
    {
        public static string ConnectionStringName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ConnectionStringName"];
            }
        }

        public static string LoggingConnectionStringName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["LoggingConnectionStringName"];
            }
        }
    }
}
