using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.SolarCloud.DataViews
{
    public class WorkflowIngressRequest
    {
        public Guid WorkflowID { get; set; }

        public string CompletionWebHookUrl { get; set; }

        public ICollection<Lead> Leads { get; set; }

        public User User { get; set; }

        public int TargetRecordCount { get; set; }

        public string FilterJson { get; set; }

        public string BoundingWKT { get; set; }

        public Guid? TerritoryID { get; set; }

        public int ExclusivityDays { get; set; }



    }
}
