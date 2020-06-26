using DataReef.Core.Infrastructure.Authorization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.FinancialIntegration
{
    [Table("AdapterRequests", Schema = "FI")]
    public class AdapterRequest : EntityBase
    {
        public AdapterRequest()
        {
            CreatedByID = SmartPrincipal.UserId;
        }

        [DataMember]
        public string AdapterName { get; set; }

        [DataMember]
        public string Request { get; set; }

        [DataMember]
        public string Response { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Headers { get; set; }

        [DataMember]
        public string Prefix { get; set; }
    }
}
