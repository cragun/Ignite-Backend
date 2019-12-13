using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SST
{
    [DataContract]
    public class SstRequestEnergyUsage
    {
        [DataMember(Name = "utility")]
        public string UtilityCompany { get; set; }
    }
}