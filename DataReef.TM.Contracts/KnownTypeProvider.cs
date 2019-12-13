using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataReef.Core.Common;
using DataReef.TM.Models;

namespace DataReef.TM.Contracts
{
    /// <summary>
    /// todo: MAJOR Refactor : get aseemblies from catalog
    /// </summary>
    internal static class KnownTypesProvider
    {
        private static readonly List<Type> KnownModelTypes = new List<Type>();

        static KnownTypesProvider()
        {
            var assemblies = AssemblyLoader.LoadAssemblies("DataReef.TM.Models.dll");

            KnownModelTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(EntityBase))).ToList();
        }

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return KnownModelTypes;
        }
    }
}
