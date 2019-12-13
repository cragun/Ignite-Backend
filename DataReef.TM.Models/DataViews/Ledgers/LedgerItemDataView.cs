using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.DataViews.Ledgers
{
    [DataContract]
    public enum LedgerItemType
    {
        [EnumMember]
        Unknown =0,
        
        [EnumMember]
        Purchase = 1,
        
        [EnumMember]
        TransferIn = 2,
        
        [EnumMember]
        TransferOut = 3,
        
        [EnumMember]
        Adjustment = 4,
        
        [EnumMember]
        Expense = 5
    }

    [DataContract]
    [NotMapped]
    public class LedgerItemDataView
    {
        [DataMember]
        public System.DateTime Date { get; set; }

        [DataMember]
        public LedgerItemType Type { get; set; }

        [DataMember]
        public double Amount { get; set; }

        [DataMember]
        public string Reference { get; set; }
    
    
    }
}
