using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Proposals
{
    public class ProposalMediaUploadRequest
    {
        /// <summary>
        /// Base64 representation of the media content
        /// </summary>
        public byte[] Content { get; set; }
        /// <summary>
        /// Item content-type
        /// </summary>
        public string ContentType { get; set; }

        public ProposalMediaItemType MediaItemType { get; set; }

        /// <summary>
        /// Optional image notes
        /// </summary>
        public string Notes { get; set; }

        public string Name { get; set; }

    }
}
