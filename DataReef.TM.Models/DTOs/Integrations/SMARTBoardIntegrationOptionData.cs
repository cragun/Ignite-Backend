
namespace DataReef.TM.Models.DTOs.Integrations
{
    public class SMARTBoardIntegrationOptionData
    {
        public string BaseUrl { get; set; }

        public string ApiKey { get; set; }

        public string HomeUrl { get; set; }

        public string CreditApplicationUrl { get; set; }
    }



    public class JobNimbusIntegrationOptionData
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string ApiKey { get; set; }

        public string HomeUrl { get; set; }
    }

}
