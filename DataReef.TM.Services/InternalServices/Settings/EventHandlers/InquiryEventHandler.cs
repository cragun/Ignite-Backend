using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.Extensions;
using DataReef.TM.Models.PubSubMessaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataReef.TM.Services.InternalServices.Settings.EventHandlers
{
    public class InquiryEventHandler : IEventHandler
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

            
            var inquiryService = ServiceLocator.Current.GetInstance<IInquiryService>();

            //set the oldDisposition field if not already set
            if (string.IsNullOrEmpty(inquiry.OldDisposition))
            {
                var latestDisposition = inquiryService.GetLatestPropertyDisposition(inquiry.PropertyID, inquiry.Guid);
                if (!string.IsNullOrEmpty(latestDisposition))
                {
                    inquiry.OldDisposition = latestDisposition;
                    inquiryService.Update(inquiry);
                }
            }


            //check if the inquiry is the first reported for the user
            if (inquiryService.IsInquiryFirstForUser(inquiry.Guid, inquiry.PersonID))
            {
                var ouSettingService = ServiceLocator.Current.GetInstance<IOUSettingService>();
                var personService = ServiceLocator.Current.GetInstance<IPersonService>();

                //if the setting exists, send an email to onboarding letting them know that user has changed their first disposition
                var settings = ouSettingService
                    .GetSettingsByPropertyID(inquiry.PropertyID);
                var email = settings
                    ?.FirstOrDefault(x => x.Name != null && x.Name == "Inquiries.Onboarding.Email");
                string emailValue = email?.Value;

                if (!string.IsNullOrEmpty(emailValue))
                {
                    var person = Task.Run(() => personService.Get(inquiry.PersonID)).Result;

                    Task.Factory.StartNew(() =>
                    {
                        var body = $"User {person.FullName} ({person.EmailAddressString}) just changed their first disposition. <br/>";
                        Mail.Library.SendEmail(emailValue, null, $"User {person.FullName} started", body, true);
                    });
                }
            }


            var conditionsPass = handlerDataView.RunConditionsForInquiry(inquiry);

            if (conditionsPass)
            {
                var sstAdapter = ServiceLocator.Current.GetInstance<ISolarSalesTrackerAdapter>();
                sstAdapter.SubmitLead(inquiry.PropertyID);
                return true;
            }

            return false;
        }

    }
}
