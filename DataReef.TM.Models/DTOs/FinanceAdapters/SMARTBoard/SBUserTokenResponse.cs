using DataReef.TM.Models.DTOs.FinanceAdapters.SST;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard
{
    [DataContract]
    public class SBUserTokenResponse
    {
        [DataMember]
        public SstResponseMessageToken message { get; set; }

        [DataMember]
        public UserTokenModel User { get; set; }
    }

    [DataContract]
    public class SstResponseMessageToken
    {
        [DataMember]
        public string text { get; set; }

        [DataMember]
        public string type { get; set; }
    }

    [DataContract]
    public class UserTokenModel
    {
        [DataMember]
        public string token { get; set; }

        [DataMember]
        public string id { get; set; }
    }
}
