using System;

namespace DataReef.Core
{
    /// <summary>
    /// Decorate a Property of an object with this attribute to tell the framework that his object defines a Sync Domain
    /// A sync domain is a domain scope.  Devices (User/Device) belong to a sync domain and all devices of a sync domain
    /// get the changes associated with that domain
    /// </summary>
    public class SyncDomainAttribute:System.Attribute
    {
        public SyncDomainAttribute(Guid guid):base()
        {
            this.Guid = guid;
        }

        public Guid Guid { get; set; }
    }
}
