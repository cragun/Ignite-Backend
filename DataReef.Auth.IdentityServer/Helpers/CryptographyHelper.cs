using System;
using System.Security.Cryptography;
using System.Text;

namespace DataReef.Auth.IdentityServer.Helpers
{
    internal static class CryptographyHelper
    {
        internal static string ComputePasswordHash(string password, string salt)
        {
            Encoding encoding = UnicodeEncoding.Unicode;

            int saltValueSize = encoding.GetByteCount(salt);
            int passwordValueSize = encoding.GetByteCount(password);

            byte[] binarySalt = encoding.GetBytes(salt);
            byte[] binaryPassword = encoding.GetBytes(password);

            // Copy the salt and the password to the hash buffer
            byte[] valueToHash = new byte[saltValueSize + passwordValueSize];
            binarySalt.CopyTo(valueToHash, 0);
            binaryPassword.CopyTo(valueToHash, saltValueSize);

            using (SHA256 sha = SHA256.Create())
            {
                byte[] computedHash = sha.ComputeHash(valueToHash);
                return Convert.ToBase64String(computedHash);
            }
        }

        internal static string GenerateSalt()
        {
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[24];
            csprng.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }

        internal static string GeneratePassword(int alphaCharactersLength, int numericCharactersLength = 0)
        {
            string alphaCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numericCharacters = "1234567890";

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (alphaCharactersLength-- > 0)
            {
                password.Append(alphaCharacters[random.Next(alphaCharacters.Length)]);
            }

            while (numericCharactersLength-- > 0)
            {
                password.Append(numericCharacters[random.Next(numericCharacters.Length)]);
            }

            return password.ToString();
        }
    }
}