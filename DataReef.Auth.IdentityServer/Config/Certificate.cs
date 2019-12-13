using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using DataReef.Auth.IdentityServer.Helpers;

namespace DataReef.Auth.IdentityServer.Config
{
    static class Certificate
    {
        public static X509Certificate2 Get()
        {
            Assembly assembly = typeof(Certificate).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(typeof(Certificate).Namespace + ".idsrv3test.pfx"))
            {
                return new X509Certificate2(StreamHelper.ReadStream(stream), "idsrv3test");
            }
        }
    }
}