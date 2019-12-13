using DataReef.TM.Models.Enums;
using System;
using System.Dynamic;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class SignDocumentRequest
    {
        public Guid ProposalID { get; set; }
        public Guid PropertyID { get; set; }
        public Guid FinancePlanID { get; set; }
        public ExpandoObject DocumentVariables { get; set; }
        public bool NeedsSalesRepSignature { get; set; }

        /// <summary>
        /// If true, the signing was done on the spot through the app; otherwise, the signing was done through e-mail
        /// </summary>
        public string EmbeddedSigning { get; set; }
    }
}
