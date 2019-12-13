using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Proposals
{
    public class MediaItemData
    {
        public string Notes { get; set; }
        public string ContentType { get; set; }
        public ProposalMediaItemType MediaItemType { get; set; }
    }
}
