using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Settings
{
    public enum AdderItemDynamicItemType
    {
        MultiSelect = 1,
        Boolean
    }

    public class AdderItemDynamicUserDataDataView
    {
        public string Name { get; set; }
        public AdderItemDynamicItemType Type { get; set; }
        public List<string> SelectedOptions { get; set; }
        public string Value { get; set; }
    }
}
