using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Extensions;
using DataReef.TM.Models.PubSubMessaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataReef.TM.Services.InternalServices.Settings.EventHandlers
{
    public class InquiryAppointmentCancelledEventHandler : IEventHandler
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
                //set the latest appoitnment to cancelled
                var propertyService = ServiceLocator.Current.GetInstance<IPropertyService>();
                var property = Task.Run(() => propertyService.Get(inquiry.PropertyID, "Appointments")).Result;
                if(property != null)
                {
                    var latestAppointment = property.GetLatestAppointment();
                    if(latestAppointment != null)
                    {
                        var appointmentService = ServiceLocator.Current.GetInstance<IAppointmentService>();
                        appointmentService.SetAppointmentStatus(latestAppointment.Guid, AppointmentStatus.Cancelled);
                    }
                }
                
            }

            //this handler should always return false because it is not updating SMARTBoard in any way
            return false;
        }

    }
}
