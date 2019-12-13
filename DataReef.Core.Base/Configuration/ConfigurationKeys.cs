using System.Configuration;

namespace DataReef.Core.Configuration
{
    public static class ConfigurationKeys
    {
        private static string _spruceUsername;
        public static string SpruceUsername
        {
            get
            {
                if (_spruceUsername == null)
                {
                    _spruceUsername = ConfigurationManager.AppSettings["FinanceProviders.Spruce.UserName"];
                }
                return _spruceUsername;
            }
        }

        private static string _sprucePassword;
        public static string SprucePassword
        {
            get
            {
                if (_sprucePassword == null)
                {
                    _sprucePassword = ConfigurationManager.AppSettings["FinanceProviders.Spruce.Password"];
                }
                return _sprucePassword;
            }
        }
    }

    /// <summary>
    /// Strongly typed General configuration keys
    /// </summary>
    public static class CloudConfigurationKeys
    {
        /// <summary>
        /// Global storage accoung
        /// </summary>
        public const string StorageAccountConnectionString = "StorageAccount.ConnectionString";
    }
}
