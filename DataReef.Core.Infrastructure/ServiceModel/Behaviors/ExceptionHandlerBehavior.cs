using DataReef.Core.Attributes;
using DataReef.Core.Infrastructure.ServiceModel.ErrorHandlers;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web.Http;
using System.Xml;

namespace DataReef.Core.Infrastructure.ServiceModel.Behaviors
{
    [Service("Exception Marshalling Service Behavior", typeof(IServiceBehavior))]
    [Service("Exception Marshalling Endpoint Behavior", typeof(IEndpointBehavior))]
    public class ExceptionMarshallingBehavior : IServiceBehavior, IEndpointBehavior
    {
        #region IEndpointBehavior Members

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime runtime)
        {
            ApplyClientBehavior(runtime);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher dispatcher)
        {
            ApplyDispatchBehavior(dispatcher.ChannelDispatcher);
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion

        #region IServiceBehavior Members

        void IServiceBehavior.AddBindingParameters(ServiceDescription service, ServiceHostBase host, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription service, ServiceHostBase host)
        {
            foreach (var channelDispatcherBase in host.ChannelDispatchers)
            {
                var dispatcher = (ChannelDispatcher)channelDispatcherBase;
                ApplyDispatchBehavior(dispatcher);
            }
        }

        void IServiceBehavior.Validate(ServiceDescription service, ServiceHostBase host)
        {
        }

        #endregion

        #region Private

        private static void ApplyClientBehavior(ClientRuntime runtime)
        {
            // Don't add a message inspector if it already exists
            if (!runtime.MessageInspectors.OfType<ExceptionMarshallingMessageInspector>().Any())
                runtime.MessageInspectors.Add(new ExceptionMarshallingMessageInspector());

        }

        private static void ApplyDispatchBehavior(ChannelDispatcher dispatcher)
        {
            // Don't add an error handler if it already exists
            if (!dispatcher.ErrorHandlers.OfType<ExceptionMarshallingErrorHandler>().Any())
                dispatcher.ErrorHandlers.Add(new ExceptionMarshallingErrorHandler());
        }

        #endregion
    }

    public class ExceptionMarshallingMessageInspector : IClientMessageInspector
    {
        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply.IsFault)
            {
                // Create a copy of the original reply to allow default processing of the message
                MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
                Message copy = buffer.CreateMessage();  // Create a copy to work with
                reply = buffer.CreateMessage();         // Restore the original message

                object faultDetail = ReadFaultDetail(copy);
                Exception exception = faultDetail as Exception;
                if (exception != null)
                {
                    if (exception.Message == @"Access is denied.")
                    {
                        throw new HttpResponseException(HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        //log the exception in the db
                        var log = Logging.LoggerFactory.Create();
                        log.Error(exception, "Exception in service call");

                        throw exception;
                    }
                }
            }
        }

        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }

        private static object ReadFaultDetail(Message reply)
        {
            const string detailElementName = "Detail";

            using (XmlDictionaryReader reader = reply.GetReaderAtBodyContents())
            {
                // Find <soap:Detail>
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName.Equals(detailElementName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        break;
                    }
                }

                // Did we find it?
                if (reader.NodeType != XmlNodeType.Element || !reader.LocalName.Equals(detailElementName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return null;
                }

                // Move to the contents of <soap:Detail>
                if (!reader.Read())
                {
                    return null;
                }

                // De-serialize the fault
                var serializer = new NetDataContractSerializer(new StreamingContext(), int.MaxValue, false, FormatterAssemblyStyle.Full, new SurrogateSelector());
                try
                {
                    return serializer.ReadObject(reader);
                }
                catch (FileNotFoundException)
                {
                    // Serialize was unable to find assembly where exception is defined 
                    return null;
                }
            }
        }
    }
}