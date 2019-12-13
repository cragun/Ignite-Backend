using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Spruce
{
    public class SpruceCallbackRequestModel
    {
        public EsignEnvelope[] EsignEnvelopes           { get; set; }

        public Funding Funding                          { get; set; }

        public FundingRequirement[] FundingRequirements { get; set; }

        public LoanTerms LoanTerms                      { get; set; }

        public Milestones Milestones                    { get; set; }

        public long QuoteNumber                         { get; set; }

        public string QuoteStatus                       { get; set; }

        public int TotalTiers                           { get; set; }

        public string EventType                         { get; set; }

        public string EnvironmentType                   { get; set; }

        public string PreviousStatus                    { get; set; }

    }
}
