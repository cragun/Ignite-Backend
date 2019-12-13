using DataReef.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Finance
{
    [DataContract]
    public enum InterestType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Regular = 1,
        [EnumMember]
        // added to the balance
        Deferred = 2,
        [EnumMember]
        // This type of interest is used when the customer pays interest, but that period is not part of the
        // TermInYears. 
        // Service Finance has a plan where the user pays Interest only for 18 months, and then the actual loan period starts for x number of years
        OutOfLoan = 3
    }

    [DataContract]
    public enum PrincipalType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Loan = 1
    }

    [DataContract]
    public enum ReductionType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        PrincipalReduction = 1,
        [EnumMember]
        TakeHome = 2,

        /// <summary>
        /// This is how Sunnova does it. 
        /// They subtract the ITC from balance in the beginning. 
        /// </summary>
        [EnumMember]
        ReduceInTheBeginning,

        /// <summary>
        /// Sunnova does this. They subtract the ITC from balance in the first 18 months
        /// but if the customer collects the ITC, Sunnova will add the ITC interest for first 18 months to the balance
        /// starting w/ month 19.
        /// </summary>
        [EnumMember]
        ReduceInTheBeginningButAccumulateInterest,

        /// <summary>
        /// Reduce the balance using the rebates incentives.
        /// </summary>
        [EnumMember]
        ReduceInTheBeginningUsingRebatesIncentives

    }

    /// <summary>
    /// This table defines the finance plan
    /// </summary>
    [Table("FinanceDetails", Schema = "finance")]
    public class FinanceDetail : EntityBase
    {
        /// <summary>
        /// guid of the financial provider
        /// </summary>
        [DataMember]
        public Guid FinancePlanDefinitionID { get; set; }

        /// <summary>
        /// Annual Percentage Rate.  Should be expressed as 3.99 not .0399
        /// </summary>
        [DataMember]
        public float Apr { get; set; }

        /// <summary>
        /// The order which the loan is applied, if multiple loans in calculation
        /// </summary>
        [DataMember]
        public int Order { get; set; }

        /// <summary>
        /// Is Principal calculated in the loan? 
        /// </summary>
        [DataMember]
        public PrincipalType PrincipalType { get; set; }

        /// <summary>
        /// Is interest calculated in the loan
        /// </summary>
        [DataMember]
        public InterestType InterestType { get; set; }

        [DataMember]
        public ReductionType ApplyReductionAfterPeriod { get; set; }

        /// <summary>
        /// True if the interest is payed from the applied incentive on the last month and not by the customer.
        /// </summary>
        [DataMember]
        public bool IsSpruced { get; set; }


        /// <summary>
        /// Term, in Months to calculate the loan.  After this expires, the next loan in the calculation will be calculated
        /// </summary>
        [DataMember]
        public int Months { get; set; }


        #region Navigation

        [DataMember]
        [ForeignKey("FinancePlanDefinitionID")]
        public FinancePlanDefinition FinancePlanDefinition { get; set; }


        #endregion
    }
}
