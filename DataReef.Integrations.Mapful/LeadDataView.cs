using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Mapful
{
    public class LeadDataView
    {
        public string RecordLocator { get; set; }

        public string StandardizedAddress { get; set; }

        public string StandardizedCityStateZip { get; set; }

        public string StandardizedName { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public string ZipFour { get; set; }

    }
}
