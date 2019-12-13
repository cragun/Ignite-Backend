using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Solar.Proposal
{
    [DataContract]
    public class PowerMetaDataDataView
    {
        [DataMember]
        public PowerMetaDataConsumptionDataView Consumption { get; set; }
    }
}
