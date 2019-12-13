using DataReef.TM.Models.DTOs.FinanceAdapters.SST;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard
{
    [DataContract]
    public class SBUserTokenResponse
    {
        [DataMember]
        public SstResponseMessage Message { get; set; }

        [DataMember]
        public UserTokenModel User { get; set; }
    }

    [DataContract]
    public class UserTokenModel
    {
        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public string Id { get; set; }
    }
}
