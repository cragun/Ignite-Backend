using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    public class ProposalMediaItemDataView
    {
        public string Url { get; set; }
        public string ThumbUrl { get; set; }
        public ProposalMediaItemDataViewType Type { get; set; }
        public string DataJSON { get; set; }

    }

    public enum ProposalMediaItemDataViewType
    {
        None = 0,
        Data_SolarSystem = 1,
        Data_SignersImage = 5,
        UserInput_Signature = 100,
        SalesRepInput_Signature = 200,
        EnergyBill = 5000,
        ThreeD_Customer_Signature = 10000,
        ThreeD_SalesRep_Signature = 10001,        
    }
}
