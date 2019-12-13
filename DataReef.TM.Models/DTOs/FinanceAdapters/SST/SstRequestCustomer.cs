using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SST
{
    [DataContract]
    public class SstRequestCustomer
    {
        [DataMember(Name = "first_name")]
        public string CustomerFirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string CustomerLastName { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "zip")]
        public string Zip { get; set; }
    }
}