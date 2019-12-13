using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Classes
{
    public class TerritoryAssignmentNotificationTemplate
    {
        public string TerritoryName { get; set; }

        public int PropertyCount { get; set; }

        public string TerritoryLink { get; set; }

        public string[] SampleHomeNames { get; set; }
    }
}
