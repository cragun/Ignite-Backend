using System;
using System.Text;

namespace DataReef.Integrations.MailChimp
{
    public static class MailChimp
    {
        public static string GetBaseAddress(string apiKey)
        {
            var index = apiKey.IndexOf("-", StringComparison.InvariantCultureIgnoreCase);
            var domain = apiKey.Substring(index + 1);

            return $"https://{domain}.api.mailchimp.com/3.0/";
        }

        public static string Md5Encrypt(string emailAddress)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(emailAddress);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (byte t in hash)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
