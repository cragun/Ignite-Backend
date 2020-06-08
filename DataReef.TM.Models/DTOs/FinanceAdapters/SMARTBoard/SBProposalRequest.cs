using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard
{
    public class SBProposalRequest
    {
        public Lead lead { get; set; }
    }
    public class Lead
    {
        public int associated_id { get; set; }

    }

}
