using DataReef.TM.Models.DataViews.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Extensions
{
    public static class DataViewsExtensions
    {
        public static bool RunConditionsForInquiry(this OUEventHandlerDataView handlerDataView, Inquiry inquiry)
        {
            var conditionsPass = true;

            if (handlerDataView.Conditions?.Any() ?? false)
            {
                var inqType = inquiry.GetType();

                foreach (var condition in handlerDataView.Conditions)
                {
                    var propValue = inqType.GetProperty(condition.Name)?.GetValue(inquiry)?.ToString();

                    if (string.IsNullOrEmpty(propValue))
                    {
                        return false;
                    }
                    switch (condition.Operator)
                    {
                        case "=":
                            conditionsPass = conditionsPass && (propValue?.Equals(condition.Value, StringComparison.OrdinalIgnoreCase) ?? false);
                            break;
                        case "!=":
                            conditionsPass = conditionsPass && (!(propValue?.Equals(condition.Value, StringComparison.OrdinalIgnoreCase)) ?? false);
                            break;
                        default:
                            break;
                    }
                }
            }

            return conditionsPass;
        }
    }
}
