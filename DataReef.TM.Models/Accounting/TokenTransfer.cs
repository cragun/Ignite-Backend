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
    public class TokenTransfer : EntityBase
    {
        /// <summary>
        /// Amount transfered in Units
        /// </summary>
        [DataMember]
        public int Amount { get; set; }

        /// <summary>
        /// purchase reference, such as Credit Card Reference
        /// </summary>
        [DataMember]
        [StringLength(200)]
        public string Notes { get; set; }

        /// <summary>
        /// The Guid of the Ledger from which the transfer was made
        /// </summary>
        [DataMember]
        public Guid FromLedgerID { get; set; }

        /// <summary>
        /// The Guid of the Ledger to which the transfer was made
        /// </summary>
        [DataMember]
        public Guid ToLedgerID { get; set; }

        /// <summary>
        /// Void the Transaction by setting to True
        /// </summary>
        [DataMember]
        public bool IsVoid { get; set; }

        #region Navigation

        /// <summary>
        /// The Token Ledger where the Transfer Came Fromm
        /// </summary>
        [ForeignKey("FromLedgerID")]
        [DataMember]
        public TokenLedger FromLedger { get; set; }

        /// <summary>
        /// The Token Ledger to where the Transfer was sent
        /// </summary>
        [ForeignKey("ToLedgerID")]
        [DataMember]
        public TokenLedger ToLedger { get; set; }


        #endregion


    }
}
