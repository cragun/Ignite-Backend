using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    public class EditPropertyAttachmentImageNotesRequest
    {
        public Guid PropertyAttachmentItemID { get; set; }

        public Guid ImageID { get; set; }

        public string Notes { get; set; }
    }
}
