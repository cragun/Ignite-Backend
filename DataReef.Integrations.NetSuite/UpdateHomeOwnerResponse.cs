using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.NetSuite
{
    public class UpdateHomeOwnerResponse
    {
        public string name       { get; set; }

        public string internalId { get; set; }

        public string externalId { get; set; }

        public string type       { get; set; }
    }
}