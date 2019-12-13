using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class ProposalMediaItemDataView
    {
        public Guid Id { get; set; }

        public string Notes { get; set; }

        public string MimeType { get; set; }

        public ProposalMediaItemDataView()
        { }

        public ProposalMediaItemDataView(ProposalMediaItem mediaItem)
        {
            Id = mediaItem.Guid;
            Notes = mediaItem.Notes;
            MimeType = mediaItem.MimeType;
        }
    }
}
