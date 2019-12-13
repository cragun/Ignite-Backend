using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Settings
{
    public class DispositionV1DataView
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public List<string> Subcategories { get; set; }
    }
}
