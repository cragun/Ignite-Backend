namespace DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker
{
    internal static class SolarTrackerResources
    {
        public const string SettingName = "Integrations.Options";
        public const string SelectedSettingName = "Integrations.Options.Selected";
        public const string JobNimbusIntegration = "JobNimbus.Integration.Options";
        public const string IncomingApiKeySettingName = "Smartboard.Incoming.ApiKey";
        //public const string SmartboardUserIdSettingName = "Integrations.SmartBoard.Id";

        public const string Name = "SolarSalesTracker";
        public const string BaseUrl = "Integrations.SMARTBoard.BaseURL";

        public const string CreateLead = "leads/create/{0}";

        //public static class OuSettings
        //{
        //    public const string Enabled = "Financing.Providers.SolarSalesTracker.Enabled";
        //    public const string ApiKey = "Financing.Providers.SolarSalesTracker.ApiKey";
        //    public const string EmailAddress = "Financing.Providers.SolarSalesTracker.EmailAddress";
        //}

        public static class Exceptions
        {
            public const string SmartBoardSettingsNotFound = "SMART Board settings not found!";
            public const string ApiKeyNotSet = "SMART Board api key not set";
            public const string ProposalDocNotFound = "Proposal document not found";
            public const string FailedToGeneratePdf = "Failed to generate pdf";
        }

    }
}
