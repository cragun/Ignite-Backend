using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.Extensions;
using DataReef.TM.Models.PubSubMessaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;

namespace DataReef.TM.Services.InternalServices.Settings.EventHandlers
{
    public class InquiryOverrideEnergyConsultantEventHandler : IEventHandler
    {

        public bool HandleEventMessage(EventMessage eventMessage, OUEventHandlerDataView handlerDataView)
        {
            switch (eventMessage.EventAction)
            {
                case EventActionType.Insert:
                case EventActionType.Update:
                    return HandleInsert(eventMessage, handlerDataView);
            }
            return false;
        }

        private bool HandleInsert(EventMessage eventMessage, OUEventHandlerDataView handlerDataView)
        {
            var inquiry = eventMessage.EventEntity as Inquiry;
            if (inquiry == null)
                return false;

            var conditionsPass = handlerDataView.RunConditionsForInquiry(inquiry);

            if (conditionsPass)
            {
                var sstAdapter = ServiceLocator.Current.GetInstance<ISolarSalesTrackerAdapter>();
                sstAdapter.SubmitLead(inquiry.PropertyID, inquiry.CreatedByID);
                return true;
            }

            return false;
        }

    }
}
