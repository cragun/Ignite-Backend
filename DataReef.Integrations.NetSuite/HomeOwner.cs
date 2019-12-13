using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataReef.Integrations.Core;

namespace DataReef.Integrations.NetSuite
{

    public enum FinancingProgram
    {
        WGSW,
        WJB
    }

    public enum AvgMonthlyUtilityBill
    {
        AGS,
        WJB
    }

    public enum PurchaseType
    {
        Undecided,
        LeaseMonthly,
        LeasePrepaid,
        Cash,
        Loan
    }

    public enum CampaignSource
    {
        PartnerLead
    }

    public enum CampaignSubCategory
    {
        Blackbird
    }

    public class HomeOwner:IIntegrationData
    {

        public string SunEdCustId           { get; set; }
        public string PartnerId             { get; set; }
        public string SalesPersonId         { get; set; }  
        public string FirstName             { get; set; } 
        public string LastName              { get; set; }
        public string CellPhone             { get; set; } 
        public string HomePhone             { get; set; } 
        public string Email                 { get; set; } 
        public string Street                { get; set; } 
        public string City                  { get; set; } 
        public string State                 { get; set; } 
        public string Zip                   { get; set; } 
        public string PartnerCustId         { get; set; } 
        public string CoHFirstName          { get; set; } 
        public string CoHLastName           { get; set; } 
        public string CoHEmail              { get; set; }  
        public string IsHomeOwner           { get; set; } 
        public string IsCustomerOrPartner   { get; set; } 
        public string FinancingProgram      { get; set; }
        public string ElectricalUtility     { get; set; } 
        public string AvgMonthlyUtilityBill { get; set; } 
        public string PurchaseType          { get; set; }
        public string CampaignSubcategory   { get; set; }
        public string CanvasserId           { get; set; }
        public string CanvasserName         { get; set; }
        //public string CampaignSource        { get; set; }
        //public string PartnerName           { get; set; }
        //public string MiddleName            { get; set; }
        
        //public string SalesRepID            { get; set; }
        //public string CompanyID             { get; set; }


        public static HomeOwner FromDataSet(DataSet ds)
        {
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];

                HomeOwner ret = new HomeOwner();
                ret.PartnerId = "225654";
                //ret.PartnerName = "Complete Solar";
                ret.SalesPersonId = "225654";

                string[] nameParts = row["Name"].ToString().Split(' ');
                if (nameParts.Length == 2)
                {
                    ret.FirstName = nameParts[0].Trim();
                    ret.LastName = nameParts[1].Trim();
                }
                if (nameParts.Length == 3)
                {
                    ret.FirstName = nameParts[0].Trim();
                    //ret.MiddleName = nameParts[1].Trim();
                    ret.LastName = nameParts[2].Trim();
                }

                ret.HomePhone = "8011234567";
                ret.CellPhone = "2061234567";
                ret.Email = "someone@somewhere.com";
                ret.Street = ((string)row["Address1"].ToString() + " " + row["Address2"].ToString()).Trim();
                ret.City = row["City"].ToString();
                ret.State = row["State"].ToString();
                ret.Zip = row["ZipCode"].ToString();
                ret.PartnerCustId = row["Guid"].ToString();
                ret.FinancingProgram = "WGSW";
                ret.PurchaseType = "Lease - Monthly";
                ret.ElectricalUtility = "PG&E- Residential(E-1)";
                ret.AvgMonthlyUtilityBill = "$401-$600";
                //ret.CanvasserId = System.Guid.NewGuid().ToString();
                //ret.CanvasserName = "Joe Salesperson";
                //ret.CampaignSource = "1 EPP - Partner Lead";
                ret.CampaignSubcategory = "Blackbird";
                return ret;

            }
            else
            {
                return null;
            }





        }

    }
}
