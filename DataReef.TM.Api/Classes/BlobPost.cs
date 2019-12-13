using DataReef.TM.Models.Enums;
using System;

namespace DataReef.TM.Api.Classes
{
    public class BlobPost
    {
        /// <summary>
        /// Is the guid of the blob you wish to save.  So that can be a Persons Guid, an Attachments Guid, or the OU's guid
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// The name of the blob to be saved. If not empty, this overrides the Guid value. The Guid is kept for backwards compatibility
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Base64 Encoding of the Image
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The MimeType of the binary
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Wether the blob is private or it has public access rights
        /// </summary>
        public BlobAccessRights AccessRights { get; set; }
    }
}