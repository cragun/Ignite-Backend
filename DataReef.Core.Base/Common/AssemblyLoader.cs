using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DataReef.Core.Common
{
    /// <summary>
    /// Static helper class that can load assemblies from disk for a given relative path
    /// </summary>
    public static class AssemblyLoader
    {
        /// <summary>
        ///  Static helper methos that can load assemblies from disk for a given relative path
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="fileFilter"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> LoadAssemblies(string relativePath, string fileFilter)
        {
            var assemblies =
               FindAssemblyFiles(relativePath, fileFilter)
                    .Select(file => Assembly.ReflectionOnlyLoadFrom(file).FullName)
                    .Select(Assembly.Load);
            return assemblies;
        }

        /// <summary>
        /// Static helper methos that can load assemblies from disk for the current configured path
        /// Expects a AssemblyRelativePath in the configuration
        /// Defaults to "bin"
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> LoadAssemblies(string filter)
        {
            return LoadAssemblies(null, filter);
        }

        private static IEnumerable<string> FindAssemblyFiles(string relativePath, string fileFilter)
        {
            //if no path is configured
            if (relativePath == null)
            {
                //default to bin directory
                var binDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
                return Directory.EnumerateFiles(System.IO.Directory.Exists(binDir) ? binDir : 
                                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""), fileFilter);
            }

            return Directory.EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath), fileFilter);
        }
    }
}
