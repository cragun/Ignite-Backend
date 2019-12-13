using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class SignDocumentResponse
    {
        public string ContractID { get; set; }
        public ICollection<SignerLink> SignerLinks { get; set; }
    }
}
