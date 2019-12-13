using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Solar.Proposal
{
    [DataContract]
    public class PowerMetaDataConsumptionDataView
    {
        [DataMember]
        public double? AnnualAverageConsumption { get; set; }
        [DataMember]
        public PowerInformationSource? AnnualAverageConsumptionSource { get; set; }

        [DataMember]
        public double? AnnualAveragePrice { get; set; }
        [DataMember]
        public PowerInformationSource? AnnualAveragePriceSource { get; set; }

        [DataMember]
        public double? MonthlyAverageConsumption { get; set; }
        [DataMember]
        public PowerInformationSource? MonthlyAverageConsumptionSource { get; set; }
        [DataMember]
        public double? MonthlyAveragePrice { get; set; }
        [DataMember]
        public PowerInformationSource? MonthlyAveragePriceSource { get; set; }
    }
}
