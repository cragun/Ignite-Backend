using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.ClientAPI
{
    public class ProposalCloneCoreParams
    {
        public List<KeyValuePair<Guid, Guid>> ProposalDataGuids { get; set; }

        public KeyValuesCrud KeyValues { get; set; }
    }
}
