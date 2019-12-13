using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace DataReef.TM.Api.Bootstrap
{
    internal class GenericHttpControllerDescriptor : HttpControllerDescriptor
    {
        private object[] _attributeCache;
        private object[] _declaredOnlyAttributeCache;

        public GenericHttpControllerDescriptor(HttpConfiguration config, string controllerName, Type controllerType)
            : base(config, controllerName, controllerType)
        {
        }

        public override Collection<T> GetCustomAttributes<T>(bool inherit)
        {
            object[] attributes;
            // Getting custom attributes via reflection is slow. 
            // But iterating over a object[] to pick out specific types is fast. 
            // Furthermore, many different services may call to ask for different attributes, so we have multiple callers. 
            // That means there's not a single cache for the callers, which means there's some value caching here.
            if (inherit)
            {
                if (_attributeCache == null)
                {
                    var controllerAttributes = ControllerType.GetCustomAttributes(true);
                    _attributeCache = ProcessRoutePrefixAttributes(controllerAttributes, ControllerName);
                }

                attributes = _attributeCache;
            }
            else
            {
                if (_declaredOnlyAttributeCache == null)
                {
                    var controllerAttributes = ControllerType.GetCustomAttributes(false);
                    _declaredOnlyAttributeCache = ProcessRoutePrefixAttributes(controllerAttributes, ControllerName);
                }

                attributes = _declaredOnlyAttributeCache;
            }

            return new Collection<T>(attributes.OfType<T>().ToList());
        }

        private static object[] ProcessRoutePrefixAttributes(IEnumerable<object> attributes, string controllerName)
        {
            var processedAttributes = new List<object>();

            foreach (var attribute in attributes)
            {
                var genericRouteAttribute = attribute as GenericRoutePrefixAttribute;
                var newAttribute = genericRouteAttribute == null ? attribute : genericRouteAttribute.ToRoutePrefixAttribute(controllerName.ToLowerInvariant());
                processedAttributes.Add(newAttribute);
            }

            return processedAttributes.ToArray();
        }
    }
}