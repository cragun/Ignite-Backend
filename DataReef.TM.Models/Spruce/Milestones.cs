using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Spruce
{
    public class Milestones
    {
        public ConsumerAcceptance ConsumerAcceptance                { get; set; }

        public ConsumerConfirmationCall ConsumerConfirmationCall    { get; set; }

        public CreditDecision CreditDecision                        { get; set; }

        public FinalCompletion FinalCompletion                      { get; set; }

        public string NextMilestone                                 { get; set; }

        public SubstantialCompletion SubstantialCompletion          { get; set; }
    }
}
