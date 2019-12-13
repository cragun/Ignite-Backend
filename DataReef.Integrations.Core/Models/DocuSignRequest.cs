using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Core.Models
{
    public class DocuSignRequest
    {
        public string PricingQuoteId    { get; set; }

        public string EmbeddedSigning   { get; set; }

        public string PurchaseType      { get; set; }

        public string FinanceProgram    { get; set; }
    }
}
