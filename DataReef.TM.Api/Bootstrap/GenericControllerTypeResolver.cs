using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;
using DataReef.TM.Api.Controllers;
using DataReef.TM.Models;

namespace DataReef.TM.Api.Bootstrap
{
    internal class GenericControllerTypeResolver : DefaultHttpControllerTypeResolver
    {
        private readonly IEnumerable<Type> _entityControllerTypes;

        public GenericControllerTypeResolver(IEnumerable<Assembly> assemblies)
        {
            _entityControllerTypes = BuildGenericControllerMappings(assemblies);
        }
        
        public override ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver)
        {
            var getControllerTypes = base.GetControllerTypes(assembliesResolver);

            foreach (var entityControllerType in _entityControllerTypes)
                if (getControllerTypes.All(c => !entityControllerType.IsAssignableFrom(c)))
                    getControllerTypes.Add(entityControllerType);

            return getControllerTypes;
        }

        /// <summary>
        ///     Create a dictionary of model types and their generic controller
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Type> BuildGenericControllerMappings(IEnumerable<Assembly> assemblies)
        {
            var generic = typeof(EntityCrudController<>);
            var modelTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(EntityBase)) && !t.IsAbstract);
            return modelTypes.Select(modelType => generic.MakeGenericType(new[] { modelType })).ToList();
        }
    }
}