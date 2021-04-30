
namespace DataReef.TM.Models.DTOs.Integrations
{
    public class SelectedIntegrationOption
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IntegrationOptionData Data { get; set; }
    }


    public class JobNimbusIntegrationOption
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public JobNimbusOptionData Data { get; set; }
    }
    
}
