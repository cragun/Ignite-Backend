using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Spruce
{
    public class EsignEnvelope
    {
        public string CompletionDate                    { get; set; }

        public string ContractDate                      { get; set; } 

        public string EnvelopeId                        { get; set; }

        public EnvelopeRecipient[] EnvelopeRecipients   { get; set; }

        public string EnvelopeStatus                    { get; set; }

        public string EnvelopeType                      { get; set; }

        public string RescissionDate                    { get; set; }
    }
}
