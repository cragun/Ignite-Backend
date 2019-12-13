using DataReef.TM.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.PRMI
{
    [Table("VelocifyRequests", Schema = "PRMI")]
    [DataContract]
    public class VelocifyRequest : EntityBase
    {
        public VelocifyRequest() : base()
        {
            Source = VelocifyRequestSourceType.LegionApp;
        }

        #region Properties

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleInitial { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string NameSuffix { get; set; }

        [DataMember]
        public DateTime? BirthDate { get; set; }

        [DataMember]
        public string Address1 { get; set; }

        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string PrimaryPhone { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public decimal TotalSolarCost { get; set; }

        [DataMember]
        public decimal SystemCost { get; set; }

        [DataMember]
        public string SolarSystemDescription { get; set; }

        [DataMember]
        public Guid ReferenceID { get; set; }

        [DataMember]
        public string DealerName { get; set; }

        [DataMember]
        [StringLength(150)]
        public string SalesRepName { get; set; }

        [DataMember]
        [StringLength(150)]
        public string SalesRepPhone { get; set; }

        /// <summary>
        /// This property will be retrieved from database, no need to send it.
        /// </summary>
        [DataMember]
        [StringLength(250)]
        public string SalesRepEmail { get; set; }

        /// <summary>
        /// This property will be retrieved from database based on PropertyID, no need to send it.
        /// </summary>
        [DataMember]
        [StringLength(250)]
        public string SalesRepCompanyName { get; set; }

        [DataMember]
        public decimal Avm { get; set; }

        [DataMember]
        public decimal IncomeTaxCredit { get; set; }


        [DataMember]
        public DateTime? OriginalMortgageStartDate { get; set; }

        [DataMember]
        public decimal OriginalMortgageOriginalBalance { get; set; }

        [DataMember]
        public decimal OriginalMortgageCurrentBalance { get; set; }

        [DataMember]
        public decimal OriginalMortgagePaymentAmount { get; set; }

        [DataMember]
        public decimal OriginalMortgageInterestRate { get; set; }

        [DataMember]
        public VelocifyRequestSourceType Source { get; set; }

        #endregion
    }
}
