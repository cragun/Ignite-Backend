using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Spruce
{
    public class Funding
    {
        public FinalFunding FinalFunding    { get; set; }

        public Tier[] Tiers                 { get; set; }
    }
}
