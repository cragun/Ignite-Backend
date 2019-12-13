using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using DataReef.TM.Api.Classes;
using DataReef.TM.Api.Controllers;
using DataReef.TM.Models;

namespace DataReef.TM.Api.Bootstrap
{
    public class GenericControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _config;
        private readonly IEnumerable<Assembly> _assemblies;
        private static IDictionary<string, HttpControllerDescriptor> _entityControllers;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="config"></param>
        public GenericControllerSelector(IEnumerable<Assembly> assemblies, HttpConfiguration config)
            : base(config)
        {
            this._config = config;
            //this.dependencyResolver = dependencyResolver;
            this._assemblies = assemblies;

            _entityControllers = BuildGenericControllerMappings();
        }

        public override HttpControllerDescriptor SelectController(System.Net.Http.HttpRequestMessage request)
        {
            HttpControllerDescriptor controller = null;
            try
            {
                controller = base.SelectController(request);
            }
            catch (HttpResponseException httpException)
            {
                if (!_entityControllers.TryGetValue(this.GetControllerName(request).ToLowerInvariant(), out controller))
                {
                    throw httpException;
                }
            }

            return controller;
        }

        /// <summary>
        /// Provide controller mappings. Also used by APIExporer.
        /// </summary>
        /// <returns></returns>
        public override IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            var controllerMappings = base.GetControllerMapping();

            foreach (var httpControllerDescriptor in _entityControllers)
                // don't register generic controllers that have the same name as a specific controller or have already been implemented by a specific controller.
                if (controllerMappings.All(c => c.Key != httpControllerDescriptor.Key && !httpControllerDescriptor.Value.ControllerType.IsAssignableFrom(c.Value.ControllerType)))

                    if (!controllerMappings.Contains(httpControllerDescriptor))
                    {
                        controllerMappings.Add(httpControllerDescriptor);
                    }

               
            

            return controllerMappings;
        }

        /// <summary>
        /// Create a dictionary of model types and their generic controller
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, HttpControllerDescriptor> BuildGenericControllerMappings()
        {
            var dictionary = new ConcurrentDictionary<string, HttpControllerDescriptor>();

            Type generic = typeof(EntityCrudController<>);

            foreach (var modelType in GetEntityModelTypes())
            {
                if (!dictionary.Keys.Contains(modelType.Name))
                {
                    var controllerType = generic.MakeGenericType(new Type[] { modelType });
                    var pluralizedModel = modelType.Name.Pluralize();
                    var descriptor = new GenericHttpControllerDescriptor(this._config, pluralizedModel, controllerType);
                    dictionary.TryAdd(pluralizedModel, descriptor);
                }
            }
            return dictionary;

        }

        private IEnumerable<Type> GetEntityModelTypes()
        {
            return this._assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(EntityBase)) && !t.IsAbstract);
        }
    }
}