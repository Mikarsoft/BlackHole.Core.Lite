using BlackHole.Logger;
using System.Security.Cryptography;
using System.Text;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class BHCryptographer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
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
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string GenerateSHA1Hash(this string plainText)
        {
            return plainText.GenerateSHA1();
        }
    }
}
