using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Settings
{
    public class AdderItemDynamicOptionDataView
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public AdderItemDynamicItemType Type { get; set; }
        public List<string> Options { get; set; }
    }
}
