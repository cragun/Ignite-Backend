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


        public static string EncryptAPI(string clearText)
        {

            string secretKey = "q7KYLwNhYz";
            try
            {
                TripleDESCryptoServiceProvider tripleDESProvider = new TripleDESCryptoServiceProvider();
                byte[] byteKey = Encoding.UTF8.GetBytes(secretKey.PadRight(24, '\0'));
                if (byteKey.Length > 24)
                {
                    byte[] bytePass = new byte[24];
                    Buffer.BlockCopy(byteKey, 0, bytePass, 0, 24);
                    byteKey = bytePass;
                }
                byte[] byteText = Encoding.UTF8.GetBytes(clearText);
                tripleDESProvider.Key = byteKey;
                tripleDESProvider.Mode = CipherMode.ECB;
                byte[] byteMessage = tripleDESProvider.CreateEncryptor().TransformFinalBlock(byteText, 0, byteText.Length);
                return Convert.ToBase64String(byteMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string DecryptApiKey(string data)
        {
            string secretKey = "q7KYLwNhYz";
            try
            {
                byte[] byteData = Convert.FromBase64String(data);
                byte[] byteKey = Encoding.UTF8.GetBytes(secretKey.PadRight(24, '\0'));
                if (byteKey.Length > 24)
                {
                    byte[] bytePass = new byte[24];
                    Buffer.BlockCopy(byteKey, 0, bytePass, 0, 24);
                    byteKey = bytePass;
                }
                TripleDESCryptoServiceProvider tripleDESProvider = new TripleDESCryptoServiceProvider();
                tripleDESProvider.Key = byteKey;
                tripleDESProvider.Mode = CipherMode.ECB;
                byte[] byteText = tripleDESProvider.CreateDecryptor().TransformFinalBlock(byteData, 0, byteData.Length);
                return Encoding.UTF8.GetString(byteText);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string getEncryptAPIKey(string apikey)
        {
            long curruntUnixTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

            string addUnixTime = apikey+ "_" + curruntUnixTime;

            string EncryptApiKey = CryptographyHelper.EncryptAPI(addUnixTime);

            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(EncryptApiKey);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            

            return returnValue;
        }

        public static string getDecryptAPIKey(string apikey)
        {
           byte[] encodedDataAsBytes  = System.Convert.FromBase64String(apikey);

            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            
            string DecyptApiKey = DecryptApiKey(returnValue);

            string[] str = DecyptApiKey.Split('_');

            string APIKEY = str[0];

            return APIKEY;
        }

        public static bool checkTime(string apikey)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(apikey);

            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);

            string DecyptApiKey = DecryptApiKey(returnValue);

            string[] str = DecyptApiKey.Split('_');

            long unixTime = long.Parse(str[1]);

            long curruntUnixTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            long time = curruntUnixTime - unixTime;

            if (time > 300)
            {
                throw new Exception("Please send valid apikey.");
            }

            return true;
        }


    }
}