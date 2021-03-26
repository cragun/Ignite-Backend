using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class SunnovaContacts
    {
        public Contacts Contacts { get; set; }
    }

    public class Contacts
    {
        public string Mailing_PostalCode { get; set; }
        public string Preferred_Contact_Method { get; set; }
        public string Mailing_State { get; set; }
        public string type { get; set; }
        public string Mailing_Country { get; set; }
        public string Mailing_Street { get; set; }
        public object Credit_Expiration_Date { get; set; }
        public string Same_as_installation_address { get; set; }
        public object Date_of_Birth { get; set; }
        public object External_Id { get; set; }
        public object Credit_Status { get; set; }
        public object Credit_Run_Date { get; set; }
        public object Last_Date_Email_Auth_Form_Sent { get; set; }
        public string On_Title { get; set; }
        public object Credit_Status_Detail { get; set; }
        public string On_Contract { get; set; }
        public string Mailing_City { get; set; }
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
