
namespace DataReef.TM.Models.DTOs.Signatures
{
    /// <summary>
    /// A signer or mail receiver of a document
    /// </summary>
    public struct Recipient
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsSender { get; set; }
    }
}
