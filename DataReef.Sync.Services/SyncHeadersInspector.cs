using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using DataReef.Core;
using DataReef.Core.Attributes;
using DataReef.Core.Infrastructure.Authorization;

namespace DataReef.Sync.Services
{
    [Service(typeof(IClientMessageInspector))]
    public class SyncHeadersInspector : IClientMessageInspector
    {
        private const string NS = "http://smartcare.com";
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.UserIDHeaderName, NS, SmartPrincipal.UserId));
            request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.DeviceIDHeaderName, NS, SmartPrincipal.DeviceId));
            request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.OUIDHeaderName, NS, SmartPrincipal.OuId));
            request.Headers.Add(MessageHeader.CreateHeader(RequestHeaders.TenantIDHeaderName, NS, SmartPrincipal.TenantId));

            return null;
        }
    }
}
