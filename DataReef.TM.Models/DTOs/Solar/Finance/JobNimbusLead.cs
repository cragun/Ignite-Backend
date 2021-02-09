using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class JobNimbusLead
    {
        public Leads lead { get; set; }
    }


    public class Leads
    {
        public string display_name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Addresses Address { get; set; }
        public Phonenum Phone { get; set; }
        public string Status { get; set; }
        public string Id { get; set; }
        public string Preferred_Contact_Method { get; set; }
        public string Preferred_Language { get; set; }
    }

    public class Addresses
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public object Latitude { get; set; }
        public object Longitude { get; set; }
    }

    public class Phonenum
    {
        public string Number { get; set; }
        public string Type { get; set; }
    }
}
