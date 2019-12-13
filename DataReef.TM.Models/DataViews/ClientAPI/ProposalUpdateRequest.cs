using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.ClientAPI
{
    public class ProposalUpdateRequest
    {
        public string Name { get; set; }

        public List<RoofPlaneEditDataView> RoofPlanes { get; set; }

        public TagsCrud Tags { get; set; }

        public KeyValuesCrud KeyValues { get; set; }
    }
}
