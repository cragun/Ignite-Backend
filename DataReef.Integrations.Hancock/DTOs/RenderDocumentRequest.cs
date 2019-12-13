using DataReef.TM.Models.DTOs.Signatures;
using System;
using System.Collections.Generic;

namespace DataReef.Integrations.Hancock.DTOs
{
    public class RenderDocumentRequest
    {
        public Guid DocumentID { get; set; }

        public List<MergeField> MergeFields { get; set; }

        public string ExternalID { get; set; }
    }
}
