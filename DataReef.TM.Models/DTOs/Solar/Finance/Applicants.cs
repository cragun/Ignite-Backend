using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class Applicants
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string otherPhone { get; set; }
        public string email { get; set; }
        public bool isPrimary { get; set; }
        public string mailingStreet { get; set; }
        public string mailingCity { get; set; }
        public string mailingStateName { get; set; }
        public string mailingZipCode { get; set; }
        public string residenceStreet { get; set; }
        public string residenceCity { get; set; }
        public string residenceStateName { get; set; }
        public string residenceZipCode { get; set; }
        public bool? isCreditRun { get; set; }
    }

    public class SystemDesigns
    {
        public string id { get; set; }
        public double azimuth { get; set; }
        public string moduleMake { get; set; }
        public string moduleMakeName { get; set; }
        public string moduleModel { get; set; }
        public int moduleCount { get; set; }
        public string inverterMake { get; set; }
        public string inverterMakeName { get; set; }
        public string inverterModel { get; set; }
        public int inverterCount { get; set; }
        public int estAnnualProductionkwh { get; set; }
        public int tilt { get; set; }
        public int systemSize { get; set; }
        public int batteryCapacity { get; set; }
        public int batteryCount { get; set; }
        public string batteryMake { get; set; }
        public string batteryMakeName { get; set; }
        public string batteryModel { get; set; }
        public string projectType { get; set; }

    }
    public class Quotes
    {
        public string id { get; set; }
        public string productName { get; set; }
        public string systemDesignId { get; set; }
        public bool isSyncing { get; set; }
        public string quoteNumber { get; set; }
        public DateTime createdDateTime { get; set; }
        public decimal loanAmount { get; set; }
        public double finalMonthlyPayment { get; set; }
        public double finalEscalatedMonthlyPayment { get; set; }
        public double monthlyPayment { get; set; }
        public double escalatedMonthlyPayment { get; set; }
        public string productType { get; set; }
        public double term { get; set; }
        public double apr { get; set; }
        public bool isACH { get; set; }

    }

}
