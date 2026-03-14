using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HyperReps.Infrastructure.Persistence.Converters
{
    public class EncryptionValueConverter : ValueConverter<string, string>
    {
        public EncryptionValueConverter(string encryptionKey) : base(
            v => Encrypt(v, encryptionKey),
            v => Decrypt(v, encryptionKey))
        {
        }

        private static string Encrypt(string plainText, string keyString)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV(); // Generate a new IV for each encryption

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            
            // Write the IV to the beginning of the stream
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        private static string Decrypt(string cipherText, string keyString)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            var fullCipher = Convert.FromBase64String(cipherText);
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aes = Aes.Create();
            aes.Key = key;

            // Read the IV from the beginning of the array
            var iv = new byte[aes.BlockSize / 8];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
    }
}
