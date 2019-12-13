using DataReef.Core.Attributes;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Accounting
{
    /// <summary>
    /// TokenLedgers represent the Token Account for a User.  Each has a Balance which is the sum of purchases + transfers in - transfers out - expenses + adjustments
    /// </summary>
    [BootstrapExcluded(BootstrapType = BootstrapType.Api)]
    public class TokenLedger : EntityBase
    {
        /// <summary>
        /// The Guid of the User
        /// </summary>
        /// 
        [DataMember]
        public Guid UserID { get; set; }

        /// <summary>
        /// The Guid of the Person.  Denormalized, yes
        /// </summary>
        /// 
        [DataMember]
        public Guid PersonID { get; set; }

        /// <summary>
        /// Virtual Field. the balance of the account
        /// </summary>
        [NotMapped]
        public Decimal Balance { get; set; }

        /// <summary>
        /// A person can have multiple Ledgers, but only ONE primary ledger.  If someone transferes anything to a person, it goes to the primary ledger if otherwise unspecified
        /// </summary>
        [DataMember]
        public bool IsPrimary { get; set; }

        #region Navigation 

        [DataMember]
        public ICollection<TokenAdjustment> Adjustments { get; set; }

        [InverseProperty("Ledger")]
        [DataMember]
        public ICollection<TokenPurchase> Purchases { get; set; }

        [DataMember]
        public ICollection<TokenExpense> Expenses { get; set; }

        [InverseProperty("ToLedger")]
        [DataMember]
        public ICollection<TokenTransfer> TransfersIn { get; set; }

        [InverseProperty("FromLedger")]
        [DataMember]
        public ICollection<TokenTransfer> TransfersOut { get; set; }

        
        #endregion

    }
}
