using System;
using System.Diagnostics;

namespace DataReef.Core.Infrastructure.ServiceModel
{
    [DebuggerDisplay("Generic: {IsGeneric} - {Address}")]
    public class WcfService
    {
        public Type ServiceType { get; set; }

        public Type ServiceContract { get; set; }

        public Type GenericBaseType { get; set; }

        public string Address { get; set; }

        public bool IsGeneric { get { return GenericBaseType != null; } }
    }
}