using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Mosaic.Models
{
    public class MosaicHomeOwner
    {
        public string FirstName             { get; set; }

        public string LastName              { get; set; }

        public string CellPhone             { get; set; }

        public string HomePhone             { get; set; }

        public string Email                 { get; set; }

        public string Street                { get; set; }

        public string City                  { get; set; }

        public string State                 { get; set; }

        public string Zip                   { get; set; }

        public string CoHFirstName          { get; set; }

        public string CoHLastName           { get; set; }

        public string CoHEmail              { get; set; }

        public string PartnerId             { get; set; }

        public string SalesPersonId         { get; set; }

        public string IsCustomerOrPartner   { get; set; }

        public string FinancingProgram      { get; set; }

        public string PurchaseType          { get; set; }

        public string ElectricalUtility     { get; set; } 

        public string AvgMonthlyUtilityBill { get; set; }

        public bool ByPassModsolar          { get; set; }

        public string SunEdCustId           { get; set; }
    }
}
