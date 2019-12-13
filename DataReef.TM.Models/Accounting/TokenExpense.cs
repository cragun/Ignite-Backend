using DataReef.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Accounting
{
    [BootstrapExcluded(BootstrapType = BootstrapType.Api)]
    public class TokenExpense : EntityBase
    {

        [DataMember]
        public Guid LedgerID { get; set; }

        /// <summary>
        /// Amount in Units spent
        /// </summary>
        [DataMember]
        public int Amount { get; set; }

        /// <summary>
        /// The guid of the batch prescreen that created the expense
        /// </summary>
        [DataMember]
        public Guid? BatchPrescreenID { get; set; }

        /// <summary>
        /// The guid of the instant prescreen that created the expense
        /// </summary>
        [DataMember]
        public Guid? InstantPrescreenID { get; set; }

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
