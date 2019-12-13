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
    [NotMapped]
    public class LedgerDataView
    {
        [DataMember]
        public double Balance { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ICollection<LedgerItemDataView> LedgerItems { get; set; }

        [DataMember]
        public Guid PersonID { get; set; }

    }
}
