using System.Runtime.Serialization;

namespace DataReef.Integrations.MailChimp.Models
{
    [DataContract]
    public class MailChimpUserDetails
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
}
