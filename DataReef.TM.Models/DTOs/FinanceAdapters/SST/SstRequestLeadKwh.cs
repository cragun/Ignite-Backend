using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SST
{
    [DataContract]
    public class SstRequestLeadKwh
    {
        [DataMember(Name = "kw_hours")]
        public decimal SystemSize { get; set; }
    }
}