using System;
namespace DataReef.Integrations.Spruce.DTOs
{
    public class GenDocsRequest 
    {
        /// <summary>
        /// Quote is the Kilowatt financial primary key used to interact with our portal. QuoteNumber is required if SponsorID not provided.
        /// </summary>
        public long QuoteNumber { get; set; }

        /// <summary>
        /// If the Quote number is not included sponsor Id is required. Partner Id must also be included.
        /// </summary>
        public string SponsorId { get; set; }

        /// <summary>
        /// This is the Partner Id provided by Kilowatt Financial.  Either Quote number or Sponsor Id must also be included.
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Total cost of the job
        /// </summary>
        public decimal TotalCashSalesPrice { get; set; }

        /// <summary>
        /// Sales Tax (0 is an acceptable value)
        /// </summary>
        public decimal SalesTax { get; set; }

        /// <summary>
        /// Down payment provided by the applicant / coapplicant (0 is an acceptable value)
        /// </summary>
        public decimal CashDownPayment { get; set; }

        /// <summary>
        /// Total amount financed.
        /// </summary>
        public decimal AmountFinanced { get; set; }

        /// <summary>
        /// Work Begin Date
        /// </summary>
        public DateTime InstallCommencementDate { get; set; }

        /// <summary>
        /// Work End Date (substantial or final)
        /// </summary>
        public DateTime SubstantialCompletionDate { get; set; }

        /// <summary>
        /// Permission to Operate date
        /// </summary>
        public DateTime ProjectedPTODate { get; set; }

        /// <summary>
        /// Applicants email address, required if the applicant email address not previously provided or is different than what was previously provided. 
        /// </summary>
        public string EmailApplicant { get; set; }

        /// <summary>
        /// Co-Applicants email address, required if the coapplicant email address not previously provided or is different than what was previously provided.
        /// </summary>
        public string EmailCoapplicant { get; set; }

        /// <summary>
        /// RIC Signors Email address.
        /// </summary>
        public string EmailAgreement { get; set; }
    }
}