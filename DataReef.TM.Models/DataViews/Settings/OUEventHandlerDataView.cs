using DataReef.TM.Models.PubSubMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Settings
{
    public class OUEventHandlerDataView
    {
        public string EventSource { get; set; }
        public EventActionType? EventAction { get; set; }

        public List<OUEventHandlerConditionDataView> Conditions { get; set; }

        /// <summary>
        /// Full name (including namespace) of the Handler class
        /// </summary>
        public string HandlerClassFullName { get; set; }

    }

    public class OUEventHandlerConditionDataView
    {
        public string Name { get; set; }
        /// <summary>
        /// "=", "!=" for now
        /// </summary>
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}
