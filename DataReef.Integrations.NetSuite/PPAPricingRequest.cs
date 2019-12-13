using DataReef.Integrations.Core;
using System;

namespace DataReef.Integrations.NetSuite
{
    public class PPAPricingRequest: IIntegrationData
    {
        public string SunEdCustId   { get; set; }

        public Quote Quote          { get; set; }

        public ArrayElement[] Array { get; set; }

        public SystemElement System { get; set; }

        public Contract Contract    { get; set; }

        public Guid FinancePlanID { get; set; }

        /// <summary>
        /// Base64 encoded string of the proposal PDF
        /// </summary>
        public string ProposalBytes { get; set; }

        /// <summary>
        /// If true, the signing was done on the spot through the app; otherwise, the signing was done through e-mail
        /// </summary>
        public string EmbeddedSigning { get; set; }
    }
}
