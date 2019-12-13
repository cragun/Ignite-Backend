using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DataReef.TM.Services.Services
{
    internal static class SerializationExtensions
    {
        public static string XmlSerialize<T>(this T obj)
        {
            if (obj == null)
                return string.Empty;

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                var writerSettings = new XmlWriterSettings { CheckCharacters = true };

                using (var sw = new StringWriter())
                {
                    using (var xmlWriter = XmlWriter.Create(sw, writerSettings))
                    {
                        serializer.Serialize(xmlWriter, obj);
                    }

                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception serializing object", ex);
            }
        }
    }
}
