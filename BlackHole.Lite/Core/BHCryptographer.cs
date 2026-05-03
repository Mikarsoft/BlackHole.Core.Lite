using System.Security.Cryptography;
using System.Text;

namespace BlackHole.Core
{
    /// <summary>
    /// Utility class providing AES encryption/decryption and MD5 hashing extensions.
    /// </summary>
    /// <remarks>
    /// All methods are extension methods on <c>string</c>. Use for encrypting/decrypting
    /// sensitive data stored in the database, or for generating MD5 hashes of passwords or keys.
    /// </remarks>
    public static class BHCryptographer
    {
        /// <summary>
        /// Encrypts a string using AES encryption with the specified key.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="key">The encryption key; will be MD5-hashed internally to create the cipher key.</param>
        /// <remarks>
        /// The encrypted result is Base64-encoded for storage in text fields.
        /// </remarks>
        /// <returns>The Base64-encoded encrypted string.</returns>
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
        /// Decrypts a Base64-encoded AES-encrypted string using the specified key.
        /// </summary>
        /// <param name="cipherText">The Base64-encoded encrypted text.</param>
        /// <param name="key">The encryption key; must match the key used during encryption.</param>
        /// <remarks>
        /// Throws an exception if the cipherText is not valid Base64 or decryption fails.
        /// </remarks>
        /// <returns>The decrypted plaintext string.</returns>
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
        /// Computes the MD5 hash of a string, returning the hexadecimal representation.
        /// </summary>
        /// <param name="input">The string to hash.</param>
        /// <remarks>
        /// MD5 is not cryptographically secure for password storage; use for other purposes like checksums.
        /// The hash is returned as a hex string suitable for database storage.
        /// </remarks>
        /// <returns>The hexadecimal MD5 hash of the input string.</returns>
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
    }
}
