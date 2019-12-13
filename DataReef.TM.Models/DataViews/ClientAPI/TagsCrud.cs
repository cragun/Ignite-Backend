using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.ClientAPI
{
    public class TagsCrud
    {
        public ICollection<string> Insert { get; set; }

        public ICollection<string> Delete { get; set; }

    }
}
