using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class SignDocumentRequestHancock
    {
        public SignDocumentRequestHancock()
        {
            MergeFields = new List<MergeField>();
            UserInputs = new List<UserInput>();
        }

        public Guid FinancePlanID { get; set; }
        public Guid ProposalTemplateID { get; set; }
        public string ContractorID { get; set; }
        public List<MergeField> MergeFields { get; set; }
        public List<UserInput> UserInputs { get; set; }
    }
}
