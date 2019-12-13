using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace DataReef.TM.Api.Security.Certificates
{
    static class Certificate
    {
        private static X509Certificate2 certificate;
        private static string CertificatePassword = ConfigurationManager.AppSettings["Certificate.Password"];

        public static X509Certificate2 Get()
        {
            if (certificate != null)
            {
                return certificate;
            }

            Assembly assembly = typeof(Certificate).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("DataReef.TM.Api.Security.Certificates.certificate.pfx"))
            {
                certificate = new X509Certificate2(ReadStream(stream), CertificatePassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
                return certificate;
            }
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }
    }
}
