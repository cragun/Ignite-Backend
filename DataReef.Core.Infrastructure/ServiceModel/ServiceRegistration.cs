using System;
using System.Diagnostics;

namespace DataReef.Core.Infrastructure.ServiceModel
{
    public class ServiceRegistration
    {
        public Type ServiceType { get; set; }

        public Type ServiceContract { get; set; }

        public Type GenericBaseType { get; set; }

        public bool IsGeneric { get { return GenericBaseType != null; } }
    }
}