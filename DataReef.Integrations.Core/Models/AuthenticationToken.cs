
namespace DataReef.Integrations.Core.Models
{
    public class AuthenticationToken
    {
        public string id            { get; set; }
        public string userId        { get; set; }
        public bool mfaActive       { get; set; }
        public string cookieToken   { get; set; }
    }
}
