using System;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.Signatures
{
    [DataContract(Name="callback", Namespace="")]
    public class Callback
    {
        [DataMember(Name="callback-type")]
        public string CallbackType { get; set; }

        [DataMember(Name="guid")]
        public string Guid { get; set; }

        [DataMember(Name="status")]
        public string Status { get; set; }

        [DataMember(Name="created-at")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name="signed-at")]
        public DateTime SignedAt { get; set; }
    }
}
