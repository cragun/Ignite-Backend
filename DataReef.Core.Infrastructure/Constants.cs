
using System;
using System.Configuration;
namespace DataReef.Core
{
    public class Constants
    {
        public static string APIBaseAddress = ConfigurationManager.AppSettings["DataReef.TM.API.BaseAddress"];
        public static string GoogleLocationApikey = ConfigurationManager.AppSettings["GoogleMaps.LocationServices.Apikey"];
        
        public static string UtilsAPIUrl = ConfigurationManager.AppSettings["DataReef.UtilsAPI.Url"];
        public static string CustomURL = ConfigurationManager.AppSettings["DataReef.TM.CustomURL"];
        public static string SmartboardURL = ConfigurationManager.AppSettings["Integrations.SMARTBoard.BaseURL"];
        public static string IPadVersionSettingName = "RequiredVersion.Ipad";
        public static string IPadMinimumVersionSettingName = "MinimumRequiredVersion.Ipad";
        public static string LoginDays = "Ignite.Portal.LoginDays";
        public static string IOSVersion = "Ignite.Portal.IOSVersion";
        public static string FromEmailName = "Ignite App";
    }

    public static class Features
    {
        public static bool Proposal_AttachPDF = Convert.ToBoolean(ConfigurationManager.AppSettings["DataReef.Features.Proposal.AttachPDF"] ?? "True");
    }

    public static class FinancialEngineSettings
    {
        public static int FederalTaxIncentiveMonth = 18;
    }

    public static class DynamicIncentive_Names
    {
        public const string TriSMART_Incentives_SubPlans = "TriSmart.Financing.Incentives.SubPlans";
        public const string TriSMART_Incentives_SummaryOnly = "TriSmart.Financing.Incentives.SummaryOnly";

        public const string Sigora_Incentives_UseInSavingsTable = "Sigora.Financing.Incentives.UseInSavingsTable";
    }

    public class RequestHeaders
    {
        public const string DeviceIDHeaderName = "DataReef-DeviceID";
        public const string UserIDHeaderName = "DataReef-UserID";
        public const string OUIDHeaderName = "DataReef-OUID";
        public const string PersonIDHeaderName = "DataReef-PersonID";
        public const string TenantIDHeaderName = "DataReef-TenantID";
        public const string AccountIDHeaderName = "DataReef-AccountID";
        public const string ClientVersionHeaderName = "DataReef-FullVersion";
        public const string DeviceDateHeaderName = "DataReef-DeviceDate";

        public const string DeviceTypeHeaderName = "DataReef-DeviceType";
    }

    public class DataReefClaimTypes
    {
        public const string UserId = "http://datareef.com/claims/userid";
        public const string DeviceId = "http://datareef.com/claims/deviceid";
        public const string OuId = "http://datareef.com/claims/ouid";
        public const string AccountID = "http://datareef.com/claims/accountid";
        public const string TenantId = "http://datareef.com/claims/tenantid";
        public const string ClientVersion = "http://datareef.com/claims/clientversion";
        public const string DeviceDate = "http://datareef.com/claims/devicedate";
        public const string DeviceType = "http://datareef.com/claims/devicetype";
    }

    public class JwtClaimTypes
    {
        public const string Subject = "sub";
        public const string TenantId = "tid";
    }

    public class SystemUsers
    {
        public const string SyncService = "1cafdd5a-1e72-43a7-8125-e071915e224c";
    }
}
