using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class JobNimbusContacts
    {
        public Contacts Contacts { get; set; }
    }

    public class Contactss
    {
        public string Mailing_PostalCode { get; set; }
        public string Preferred_Contact_Method { get; set; }
        public string Mailing_State { get; set; }
        public string type { get; set; }
        public object Date_of_Birth { get; set; }
        public object External_Id { get; set; }
        public string Role { get; set; }
        public string Credit_Check_Authorized { get; set; }
        public string Full_Name { get; set; }
        public string Phone { get; set; }
        public string Preferred_Language { get; set; }
        public string First_Name { get; set; }
        public string Id { get; set; }
        public string Phone_Type { get; set; }
        public object Email { get; set; }
        public string Last_Name { get; set; }
    }
}
