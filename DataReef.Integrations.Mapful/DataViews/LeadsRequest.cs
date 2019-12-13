using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Mapful.DataViews
{
    public class LeadsRequest: AnalyzeRequest
    {
        public int RecordCount { get; set; }

        public System.DateTime? ExpirationDate { get; set; }
    }
}
