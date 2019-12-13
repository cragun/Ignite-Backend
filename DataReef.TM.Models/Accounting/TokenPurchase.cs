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
    /// Used to record actual token purchases, real money
    /// </summary>
    [BootstrapExcluded(BootstrapType = BootstrapType.Api)]
    public class TokenPurchase : EntityBase
    {
        /// <summary>
        /// Amount purchases in Units
        /// </summary>
        /// 
        [DataMember]
        public int Amount { get; set; }

        /// <summary>
        /// Price in Dollars paid for the purchase
        /// </summary>
        [DataMember]
        public decimal Price {get;set;}

        /// <summary>
        /// purchase reference, such as Credit Card Reference
        /// </summary>
        [DataMember]
        [StringLength(100)]
        public string Reference { get; set; }

        /// <summary>
        /// The Guid of the Ledger for which the purchase was made
        /// </summary>
        [DataMember]
        public Guid LedgerID { get; set; }

        #region Navigation

        /// <summary>
        /// The Token Ledger
        /// </summary>
        /// 
        [ForeignKey("LedgerID")]
        [DataMember]
        public TokenLedger Ledger { get; set; }

        #endregion


    }
}
