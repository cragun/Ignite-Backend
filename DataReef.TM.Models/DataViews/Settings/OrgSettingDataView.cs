using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Settings
{
    public class OrgSettingDataView
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ValueType => ((int)_valueType).ToString();
        public string DetailName { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }

        protected SettingValueType _valueType;

        public OrgSettingDataView() { }

        public OrgSettingDataView(SettingValueType valueType)
        {
            _valueType = valueType;
        }

        public int? IntValue()
        {
            int result = 0;
            if (int.TryParse(Value, out result))
            {
                return result;
            }
            return null;
        }

        public double? DoubleValue()
        {
            double result = 0;
            if (double.TryParse(Value, out result))
            {
                return result;
            }
            return null;
        }
    }
}
