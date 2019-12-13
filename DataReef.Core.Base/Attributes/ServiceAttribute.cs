using System;

namespace DataReef.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceAttribute : Attribute
    {
        /// <summary>
        ///   Gets the name of the service.
        /// </summary>
        /// <value> The name of the service. </value>
        public string ServiceName { get; private set; }

        /// <summary>
        ///   Gets the type of the export.
        /// </summary>
        /// <value> The type of the export. </value>
        public Type ServiceType { get; private set; }

        /// <summary>
        ///   Gets the scope of the export.
        /// </summary>
        public ServiceScope Scope { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ServiceAttribute" /> class.
        /// </summary>
        public ServiceAttribute()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ServiceAttribute" /> class.
        /// </summary>
        /// <param name="serviceType"> Type of the export. </param>
        public ServiceAttribute(Type serviceType)
            : this(null, serviceType)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ServiceAttribute" /> class.
        /// </summary>
        /// <param name="serviceName"> Name of the service. </param>
        public ServiceAttribute(string serviceName)
            : this(serviceName, null)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ServiceAttribute" /> class.
        /// </summary>
        /// <param name="scope"> The scope of the service. </param>
        public ServiceAttribute(ServiceScope scope)
            : this(null, null, scope)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ServiceAttribute" /> class.
        /// </summary>
        /// <param name="serviceName"> Name of the service. </param>
        /// <param name="scope"> The scope of the service. </param>
        public ServiceAttribute(string serviceName, ServiceScope scope)
            : this(serviceName, null, scope)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ServiceAttribute" /> class.
        /// </summary>
        /// <param name="serviceName"> Name of the service. </param>
        /// <param name="serviceType"> Type of the export. </param>
        public ServiceAttribute(string serviceName, Type serviceType)
            : this(serviceName, serviceType, ServiceScope.None)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ServiceAttribute" /> class.
        /// </summary>
        /// <param name="serviceType"> Type of the export. </param>
        /// <param name="scope"> The scope of the service. </param>
        public ServiceAttribute(Type serviceType, ServiceScope scope)
            : this(null, serviceType, scope)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ServiceAttribute" /> class.
        /// </summary>
        /// <param name="serviceName"> Name of the service. </param>
        /// <param name="serviceType"> Type of the export. </param>
        /// <param name="scope"> The scope of the service. </param>
        public ServiceAttribute(string serviceName, Type serviceType, ServiceScope scope)
        {
            this.ServiceName = serviceName;
            this.ServiceType = serviceType;
            this.Scope = scope;
        }
    }
}
