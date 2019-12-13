using DataReef.Core.Attributes;
using DataReef.Core.Infrastructure.Authorization;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DataReef.Core.Infrastructure.Security
{
    [Service("UserClaimsBehaviour", typeof(IEndpointBehavior))]
    public class UserClaimsClientInspector : BehaviorExtensionElement, IClientMessageInspector, IEndpointBehavior
    {
        private const string NS = "http://datareef.com";

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            try
            {
                request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.UserIDHeaderName, NS, SmartPrincipal.UserId));
                request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.DeviceIDHeaderName, NS, SmartPrincipal.DeviceId));
                request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.OUIDHeaderName, NS, SmartPrincipal.OuId));
                request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.TenantIDHeaderName, NS, SmartPrincipal.TenantId));
                request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.AccountIDHeaderName, NS, SmartPrincipal.AccountID));
                request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.ClientVersionHeaderName, NS, SmartPrincipal.ClientVersion));
                request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.DeviceDateHeaderName, NS, SmartPrincipal.DeviceDate));
                request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.DeviceTypeHeaderName, NS, SmartPrincipal.DeviceType));
            }
            catch
            {

            }

            return null;
        }

        public override Type BehaviorType
        {
            get { return typeof(UserClaimsClientInspector); }
        }

        protected override object CreateBehavior()
        {
            return new UserClaimsClientInspector();
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {

        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new UserClaimsClientInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }
    }
}
