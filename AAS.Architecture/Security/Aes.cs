using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AAS.Architecture.Security
{
    public static class AES
    {
        public static string Decrypt(string text, string keyString, string ivString, CipherMode mode, PaddingMode paddingMode)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var fullCipher = Convert.FromBase64String(text);

            var key = Encoding.UTF8.GetBytes(keyString);//same key string
            var iv = Encoding.UTF8.GetBytes(ivString);

            using (var aes = Aes.Create())
            {
                aes.Padding = paddingMode;
                aes.Mode = mode;
                aes.IV = iv;
                aes.Key = key;
                using (var decryptor = aes.CreateDecryptor(key, iv))
                {
                    using (var resultStream = new MemoryStream())
                    {
                        using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
                        using (var plainStream = new MemoryStream(fullCipher))
                        {
                            plainStream.CopyTo(aesStream);
                        }

                        return Encoding.UTF8.GetString(resultStream.ToArray());
                    }
                }
            }
        }

        public static string Encrypt(string encryptedText, string keyString, string ivString, CipherMode mode, PaddingMode paddingMode)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                return encryptedText;

            var buffer = Encoding.UTF8.GetBytes(encryptedText);
            var key = Encoding.UTF8.GetBytes(keyString);//same key string
            var iv = Encoding.UTF8.GetBytes(ivString);

            using (var aes = Aes.Create())
            {
                aes.Padding = paddingMode;
                aes.Mode = mode;
                aes.IV = iv;
                aes.Key = key;
                using (var encryptor = aes.CreateEncryptor(key, iv))
                {
                    using (var resultStream = new MemoryStream())
                    {
                        using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                        using (var plainStream = new MemoryStream(buffer))
                        {
                            plainStream.CopyTo(aesStream);
                        }

                        return Convert.ToBase64String(resultStream.ToArray());
                    }
                }
            }
        }
    }
}
