using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Routing;

namespace DataReef.TM.Api.Bootstrap
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class GenericRoutePrefixAttribute : Attribute
    {
        public GenericRoutePrefixAttribute()
        {
            Prefix = string.Empty;
        }

        public GenericRoutePrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }

        /// <summary>
        /// Gets the route prefix.
        /// </summary>
        public virtual string Prefix { get; private set; }

        public virtual RoutePrefixAttribute ToRoutePrefixAttribute(string controllerName)
        {
            var processedTemplate = Prefix.Replace("{controller}", controllerName);
            return new RoutePrefixAttribute(processedTemplate);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    internal class GenericRouteAttribute : Attribute, IDirectRouteFactory
    {
        public GenericRouteAttribute()
        {
            Template = String.Empty;
        }

        public GenericRouteAttribute(string template)
        {
            Template = template;
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public int Order { get; set; }

        /// <inheritdoc />
        public string Template { get; private set; }

        RouteEntry IDirectRouteFactory.CreateRoute(DirectRouteFactoryContext context)
        {
            Contract.Assert(context != null);

            var action = context.Actions.FirstOrDefault();
            var processedTemplate = action != null ? Template.Replace("{controller}", action.ControllerDescriptor.ControllerName.ToLowerInvariant()) : Template;

            IDirectRouteBuilder builder = context.CreateBuilder(processedTemplate);
            Contract.Assert(builder != null);

            builder.Name = Name;
            builder.Order = Order;
            return builder.Build();
        }
    }
}