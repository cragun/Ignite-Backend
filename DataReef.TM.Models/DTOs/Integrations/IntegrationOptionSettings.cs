using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Integrations
{
    public class IntegrationOptionSettings
    {
        public ICollection<SelectedIntegrationOption> SelectedIntegrations { get; set; }

        public ICollection<IntegrationOption> Options { get; set; }
    }
}
