
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.Signatures
{
    [DataContract]
    public enum ContentType
    {
        [EnumMember]
        String = 0,
        [EnumMember]
        Image = 1
    }

    public class MergeField
    {
        public string Name { get; set; }

        /// <summary>
        /// can be a string for text, or a base64 encoded image
        /// </summary>
        public string Value { get; set; }

        public ContentType ContentType { get; set; }

    }
}
