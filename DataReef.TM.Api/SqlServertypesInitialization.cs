using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(DataReef.TM.Api.SqlServertypesInitialization), "Init")]
namespace DataReef.TM.Api
{
    public static class SqlServertypesInitialization
    {
        public static void Init()
        {
            // Need to explicitly load SqlServerTypes native libraries
            try
            {
                var binDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
                SqlServerTypes.Utilities.LoadNativeAssemblies(binDir);
            }
            catch { }
        }
    }
}