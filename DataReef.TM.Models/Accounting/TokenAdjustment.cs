using DataReef.Core.Attributes;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Accounting
{
    [BootstrapExcluded(BootstrapType=BootstrapType.Api)]
    public class TokenAdjustment:EntityBase
    {

        [DataMember]
        public Guid LedgerID { get; set; }

        /// <summary>
        /// Amount in Units added or subtracted
        /// </summary>
        [DataMember]
        public int Amount { get; set; }

        /// <summary>
        /// Any notes for the user to reference the Expense
        /// </summary>
        [DataMember]
        [StringLength(200)]
        public string Notes { get; set; }

        /// <summary>
        /// Void the Expense
        /// </summary>
        [DataMember]
        public bool IsVoid { get; set; }

        [ForeignKey("LedgerID")]
        [DataMember]
        public TokenLedger Ledger { get; set; }

    }
}
