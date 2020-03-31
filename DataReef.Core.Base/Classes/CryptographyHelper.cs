using System;
using System.Security.Cryptography;
using System.Text;

namespace DataReef.Auth.Helpers
{
    public static class CryptographyHelper
    {
        public const int randomInitializationVectorLength = 24;

        public static string ComputePasswordHash(string password, string salt)
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

        public static string GenerateSalt()
        {
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[24];
            csprng.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }

        public static string GeneratePassword(int alphaCharactersLength, int numericCharactersLength = 0)
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

        public static string Encrypt(string text, string key, string initializationVector = "")
        {
            bool generateRandomInitializationVector = false;
            if (String.IsNullOrEmpty(initializationVector))
            {
                generateRandomInitializationVector = true;
                initializationVector = GeneratePassword(randomInitializationVectorLength);
            }

            SymmetricAlgorithm algorithm = TripleDES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(initializationVector));
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            var encryptedString = Convert.ToBase64String(outputBuffer);
            if (generateRandomInitializationVector) encryptedString = initializationVector + encryptedString;
            return encryptedString;
        }

        public static string Decrypt(string text, string key, string initializationVector = "")
        {
            if (String.IsNullOrEmpty(initializationVector))
            {
                initializationVector = text.Substring(0, randomInitializationVectorLength);
                text = text.Substring(randomInitializationVectorLength);
            }
            SymmetricAlgorithm algorithm = TripleDES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(initializationVector));
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }

        public static string DecryptApiKey(string DecryptText)
        {
            string keyValue = "q7KYLwNhYzVccCjc8S0pyYHu4izm2squk7NOPN5Y";

            byte[] SrctArray;
            byte[] DrctArray = Convert.FromBase64String(DecryptText);
            SrctArray = UTF8Encoding.UTF8.GetBytes(keyValue);
            TripleDESCryptoServiceProvider objt = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objmdcript = new MD5CryptoServiceProvider();
            SrctArray = objmdcript.ComputeHash(UTF8Encoding.UTF8.GetBytes(keyValue));
            objmdcript.Clear();
            objt.Key = SrctArray;
            objt.Mode = CipherMode.ECB;
            objt.Padding = PaddingMode.PKCS7;
            ICryptoTransform crptotrns = objt.CreateDecryptor();
            byte[] resArray = crptotrns.TransformFinalBlock(DrctArray, 0, DrctArray.Length);
            objt.Clear();
            return UTF8Encoding.UTF8.GetString(resArray);
        }


        public static string EncryptAPI(string Encryptval)
        {

            string keyValue = "q7KYLwNhYzVccCjc8S0pyYHu4izm2squk7NOPN5Y";

            byte[] SrctArray;
            byte[] EnctArray = UTF8Encoding.UTF8.GetBytes(Encryptval);
            SrctArray = UTF8Encoding.UTF8.GetBytes(keyValue);
            TripleDESCryptoServiceProvider objt = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objcrpt = new MD5CryptoServiceProvider();
            SrctArray = objcrpt.ComputeHash(UTF8Encoding.UTF8.GetBytes(keyValue));
            objcrpt.Clear();
            objt.Key = SrctArray;
            objt.Mode = CipherMode.ECB;
            objt.Padding = PaddingMode.PKCS7;
            ICryptoTransform crptotrns = objt.CreateEncryptor();
            byte[] resArray = crptotrns.TransformFinalBlock(EnctArray, 0, EnctArray.Length);
            objt.Clear();
            return Convert.ToBase64String(resArray, 0, resArray.Length);

        }

    }
}