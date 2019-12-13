using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Settings
{
    public class EnableableSettingDataView
    {
        public Guid Id { get; set; }
        public string IsDefault { get; set; }
        public string IsEnabled { get; set; }

        public EnableableSettingDataView() { }

        public EnableableSettingDataView(Guid id, int index)
        {
            Id = id;
            IsDefault = index == 0 ? "1" : "0";
            IsEnabled = "1";
        }

    }
}
