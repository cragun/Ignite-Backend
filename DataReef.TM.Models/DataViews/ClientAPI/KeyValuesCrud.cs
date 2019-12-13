using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.ClientAPI
{
    public class KeyValuesCrud
    {
        public ICollection<Models.KeyValue> Insert { get; set; }
        public ICollection<Models.KeyValue> Update { get; set; }
        public ICollection<Guid> Delete { get; set; }

    }
}
