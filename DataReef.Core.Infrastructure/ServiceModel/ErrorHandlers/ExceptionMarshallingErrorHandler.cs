using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace DataReef.Core.Infrastructure.ServiceModel.ErrorHandlers
{
    public class ExceptionMarshallingErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            // Only handle exceptions that are not already FaultExceptions
            return !(error is FaultException);
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            // Generate fault message manually
            var serializer = new NetDataContractSerializer(new StreamingContext(), int.MaxValue, false, FormatterAssemblyStyle.Full, new SurrogateSelector());
            var messageFault = MessageFault.CreateFault(new FaultCode("GenericError"), new FaultReason(error.Message), error, serializer);
            fault = Message.CreateMessage(version, messageFault, null);
        }
    }
}