using DataReef.Core.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.DataViews.Ledgers
{
    [DataContract]
    [NotMapped]
    public class TransferDataCommand
    {
        [DataMember]
        public Guid ToPersonID { get; set; }

        [DataMember]
        public int Amount { get; set; }

        [StringLength(200)]
        [DataMember]
        public string Reference { get; set; }

        [DataMember]
        public SaveResult SaveResult { get; set; }

    }
}
