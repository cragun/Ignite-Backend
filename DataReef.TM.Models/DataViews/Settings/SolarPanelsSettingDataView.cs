using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Settings
{
    public class SolarPanelsSettingDataView : EnableableSettingDataView
    {
        public SolarPanelsSettingDataView() : base() { }


        public SolarPanelsSettingDataView(Guid id, int index) : base(id, index)
        {
            PricePerWatt = 5;
        }

        public double PricePerWatt { get; set; }
    }
}
