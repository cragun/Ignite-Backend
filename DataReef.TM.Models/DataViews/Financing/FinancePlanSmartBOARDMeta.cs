using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews.Financing
{
    public class FinancePlanSmartBOARDMeta
    {
        /// <summary>
        /// SmartBOARD provided lender ID
        /// </summary>
        public string LenderID { get; set; }

        public IEnumerable<SmartBOARDCreditCheck> SmartBoardCreditCheckUrls { get; set; }
    }

    [NotMapped]
    [DataContract]
    public class SmartBOARDCreditCheck
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string CreditCheckUrl { get; set; }

        [DataMember]
        public bool UseSMARTBoardAuthentication { get; set; }

        [DataMember]
        public string SMARTBoardRootUrl { get; set; }
    }
}
