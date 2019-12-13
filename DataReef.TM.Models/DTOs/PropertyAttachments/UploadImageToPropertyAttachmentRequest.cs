using DataReef.TM.Models.DataViews.Geo;
using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    public class UploadImageToPropertyAttachmentRequest
    {
        /// <summary>
        /// ID of the property attachment for which to upload the images
        /// </summary>
        public Guid PropertyAttachmentID { get; set; }

        /// <summary>
        /// Optional parameter. If present, the images will be linked to the specified ID. If not present, the attachment item will be identified or created through SectionID and ItemID
        /// </summary>
        public Guid? PropertyAttachmentItemID { get; set; }

        /// <summary>
        /// Optional parameter. Will be used to identify attachment item if ID is not present. If attachment item is not identified, one will be created from this and ItemID
        /// </summary>
        public string SectionID { get; set; }

        /// <summary>
        /// Optional parameter. Will be used to identify attachment item if ID is not present. If attachment item is not identified, one will be created from this and SectionID
        /// </summary>
        public string ItemID { get; set; }

        public GeoPoint Location { get; set; }

        public List<string> Images { get; set; }

        public List<ImageBase64WithNotes> ImagesWithNotes { get; set; }
    }
}
