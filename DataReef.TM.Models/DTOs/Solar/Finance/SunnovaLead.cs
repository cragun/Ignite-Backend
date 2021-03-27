using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class SunnovaLead
    {
        public Lead lead { get; set; }
    }

    public class Lead
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Middle_Name { get; set; }
        public string Suffix { get; set; }
        public string Email { get; set; }
        public string Preferred_Contact_Method { get; set; }
        public string Preferred_Language { get; set; }
        public Phone Phone { get; set; }
        public Addresss Address { get; set; }
        public string LeadSource { get; set; }
        public bool Address_Validated { get; set; }
        public string ContactId { get; set; }
        public string External_Id { get; set; }
        public string Id { get; set; }


        public string Status { get; set; }
        public bool Is_Converted { get; set; }
        public bool Continue_without_Title_Validation { get; set; }
        public bool Is_Change_Order { get; set; }
        public bool Lead_System_Exists { get; set; }
        public bool Title_Verified { get; set; }
        public string Days_Since_Credit_Run { get; set; }
        public string Salesperson_Id { get; set; }
        public object Installation_Dealer_Name { get; set; }
        public string Salesperson_Name { get; set; }
        public string Dealer_Account_Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }

    public class Phone
    {
        public string Number { get; set; }
        public string Type { get; set; }
    }

    public class Addresss
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public object Latitude { get; set; }
        public object Longitude { get; set; }
    }

    public class Contact_Id
    {
        public string id { get; set; }
    }

    public class SunnovaLeadCredit
    {
        public Contact_Id Contacts { get; set; }
    }

    public class SunnovaLeadCreditResponse
    {
        public string Contact_Id { get; set; }
        public string Message { get; set; }
        public object Signing_URL { get; set; }
    }

    public class SunnovaLeadCreditRequestData
    {
        public object Return_URL;
    }

    public class SunnovaLeadCreditResponseData
    {
        public string Contact_Id { get; set; }
        public string Message { get; set; }
        public object Signing_URL { get; set; }
    }
}
