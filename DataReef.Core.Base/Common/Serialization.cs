using System.IO;
using System.Xml.Serialization;

namespace DataReef.Core.Common
{
    /// <summary>
    /// Helper class for serialization
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Extension methow for serializing a serializable object to an XML string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize">the serializable object</param>
        /// <returns></returns>
        public static string SerializeToXmlString<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, toSerialize);
                return stringWriter.ToString();
            }
        }

        public static T Deserialize<T>(this string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            T result;
            using (var reader = new StringReader(xml))
            {
                result = (T)xmlSerializer.Deserialize(reader);
            }

            return result;
        }
    }
}
