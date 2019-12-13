using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.FinanceAdapters
{
    [DataContract]
    public class SubmitApplicationResponse
    {
        [DataMember]
        //  Default 0 = no errors
        public string Error { get; set; }

        [DataMember]
        public string AppId { get; set; }

        [DataMember]
        //  Review, Approved, Declined
        public string Decision { get; set; }
    }
}
