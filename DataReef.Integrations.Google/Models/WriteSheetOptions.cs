using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Google.Models
{
    public class WriteSheetOptions
    {
        public string SheetID { get; set; }

        public List<object> Data { get; set; }
    }
}
