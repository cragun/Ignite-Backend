using System.ServiceModel.Channels;

namespace DataReef.Core.Infrastructure.ServiceModel.Bindings
{
    public class BinaryHttpBinding : Binding
    {
        public override BindingElementCollection CreateBindingElements()
        {
            var httpTransportBindingElement = new HttpTransportBindingElement
            {
                AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous,
                MaxReceivedMessageSize = long.MaxValue,
                MaxBufferSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                TransferMode = System.ServiceModel.TransferMode.StreamedResponse
            };
            var binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement
            {
                ReaderQuotas =
                {
                    MaxStringContentLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxArrayLength = int.MaxValue,
                    MaxDepth = int.MaxValue
                }
            };
            var bindingElementCollection = new BindingElementCollection
            {
                binaryMessageEncodingBindingElement,
                httpTransportBindingElement
            };
            return bindingElementCollection;
        }

        public override string Scheme
        {
            get { return "http"; }
        }
    }
}
