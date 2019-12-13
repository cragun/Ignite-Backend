using DataReef.TM.Models.Enums;

namespace DataReef.TM.Models.DTOs.Integrations
{
    public class IntegrationOption
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IntegrationType Type { get; set; }

        public bool Enabled { get; set; }

        public IntegrationOptionData Data { get; set; }
    }
}
