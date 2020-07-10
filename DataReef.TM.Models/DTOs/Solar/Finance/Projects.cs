using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class Projects
    {

        public double term { get; set; }
        public double apr { get; set; }
        public string installStreet { get; set; }
        public string installCity { get; set; }
        public string installStateName { get; set; }
        public string installZipCode { get; set; }
        public bool sendLinkEmail { get; set; }
        public List<Applicants> applicants { get; set; }
        public List<Quotes> quotes { get; set; }

        public string id { get; set; }
        public string hashId { get; set; }
        public string ownerEmail { get; set; }
        public string message { get; set; }
        public bool? isACH { get; set; }
        public bool? isCreditAuthorized { get; set; }
        public string projectStatus { get; set; }
        public string ownerName { get; set; }
        public string installerName { get; set; }
        public string projectCategory { get; set; }
        public decimal? requestedLoanAmount { get; set; }
        public decimal? amountDrawn { get; set; }
        public decimal? amountRemaining { get; set; }
        public bool? approvedForPayments { get; set; }
        public bool? blockDraw { get; set; }
        public decimal? maxAvailable { get; set; }
        public int? drawCount { get; set; }
        public bool? drawExpired { get; set; }
        public string statusText { get; set; }
        public bool? drawAutoApproved { get; set; }
    }

    public class Products
    {
        public string loanType { get; set; }
        public double term { get; set; }
        public double apr { get; set; }
        public bool? isACH { get; set; }
        public string stateName { get; set; }
    }


}
