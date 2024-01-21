using BlackHole.Logger;
using System.Security.Cryptography;
using System.Text;

namespace BlackHole.Lite.Core
{
    public static class BHCryptographer
    {
        public static string EncryptString(this string plainText, string key)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                string md5Gen = key.CreateMD5Hash();
                aes.Key = Encoding.UTF8.GetBytes(md5Gen);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(this string cipherText, string key)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            string result = string.Empty;

            using (Aes aes = Aes.Create())
            {
                string md5Gen = key.CreateMD5Hash();
                aes.Key = Encoding.UTF8.GetBytes(md5Gen);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            result = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            return result;
        }

        public static string CreateMD5Hash(this string input)
        {
            string result = string.Empty;

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                result = Convert.ToHexString(hashBytes);
            }

            return result;
        }

        public static string GenerateSHA1Hash(this string plainText)
        {
            return plainText.GenerateSHA1();
        }
    }
}
