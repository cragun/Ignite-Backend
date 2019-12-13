using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.PubSubMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Services.InternalServices.Settings.EventHandlers
{
    public interface IEventHandler
    {
        bool HandleEventMessage(EventMessage eventMessage, OUEventHandlerDataView handlerDataView);
    }
}
