namespace DataReef.TM.Services.Services.FinanceAdapters.ServiceFinance
{
    internal static class ServiceFinanceResources
    {
        public const string Name = "ServiceFinance";
        public const string MasterKey = "Financing.Providers.ServiceFinance.MasterKey";
        public const string Authenticate = "Authenticate";
        public const string SubmitApplication = "SubmitApplication";

        public static class Headers
        {
            public const string DatareefKey = "datareefkey";
            public const string DealerId = "dealerid";
        }

        public static class OuSettings
        {
            public const string SettingName = "Financing.Providers.ServiceFinance.Settings";
        }

        public static class Exceptions
        {
            public const string SettingsNotFound = "Service finance base url not found";
            public const string InvalidSetting = "Invalid setting: {0}";
            public const string InvalidPlanDefinition = "Invalid plan definition";
            public const string InvalidPlanDefinitionSetting = "Invalid plan definition setting";
        }
    }
}
