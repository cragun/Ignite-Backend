using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Spruce
{
    [Table("GenDocsRequests", Schema = "Spruce")]
    public class GenDocsRequest : EntityBase
    {
        /// <summary>
        /// Quote is the Kilowatt financial primary key used to interact with our portal. QuoteNumber is required if SponsorID not provided.
        /// </summary>
        [DataMember]
        public long QuoteNumber { get; set; }

        /// <summary>
        /// If the Quote number is not included sponsor Id is required. Partner Id must also be included.
        /// </summary>
        [DataMember]
        public string SponsorId { get; set; }

        /// <summary>
        /// This is the Partner Id provided by Kilowatt Financial.  Either Quote number or Sponsor Id must also be included.
        /// </summary>
        [DataMember]
        public string PartnerId { get; set; }

        /// <summary>
        /// Total cost of the job
        /// </summary>
        [DataMember]
        public decimal TotalCashSalesPrice { get; set; }

        /// <summary>
        /// Sales Tax (0 is an acceptable value)
        /// </summary>
        [DataMember]
        public decimal SalesTax { get; set; }

        /// <summary>
        /// Down payment provided by the applicant / coapplicant (0 is an acceptable value)
        /// </summary>
        [DataMember]
        public decimal CashDownPayment { get; set; }

        /// <summary>
        /// Total amount financed.
        /// </summary>
        [DataMember]
        public decimal AmountFinanced { get; set; }

        /// <summary>
        /// Work Begin Date
        /// </summary>
        [DataMember]
        public DateTime InstallCommencementDate { get; set; }

        /// <summary>
        /// Work End Date (substantial or final)
        /// </summary>
        [DataMember]
        public DateTime SubstantialCompletionDate { get; set; }

        /// <summary>
        /// Permission to Operate date
        /// </summary>
        [DataMember]
        public DateTime ProjectedPTODate { get; set; }

        /// <summary>
        /// Applicants email address, required if the applicant email address not previously provided or is different than what was previously provided. 
        /// </summary>
        [DataMember]
        public string EmailApplicant { get; set; }

        /// <summary>
        /// Co-Applicants email address, required if the coapplicant email address not previously provided or is different than what was previously provided.
        /// </summary>
        [DataMember]
        public string EmailCoapplicant { get; set; }

        /// <summary>
        /// RIC Signors Email address.
        /// </summary>
        [DataMember]
        public string EmailAgreement { get; set; }



        #region Navigation

        [DataMember]
        [Required]
        public QuoteRequest QuoteRequest { get; set; }

        #endregion
    }
}
