using System;

namespace DataReef.Core.Infrastructure.ServiceModel
{
    public class Proxy
    {
        public Type ServiceContract { get; set; }

        public Type GenericBaseType { get; set; }

        public string ServiceName { get; set; }

        public string BaseAddress { get; set; }
    }
}
