using System.Security.Cryptography;
using System.Text;

namespace UMS.Helpers
{
    public class EncryptionHelper(IConfiguration configuration)
    {
        private readonly string _encKey = configuration["KEYS:EncryptionKey"] ?? throw new ArgumentNullException("Key Not Found");
        private readonly string _encIV = configuration["KEYS:EncryptionIV"] ?? throw new ArgumentNullException("IV Not Found");

        // Replace with your actual IV
        public string Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encKey);
            aes.IV = Encoding.UTF8.GetBytes(_encIV);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
            using StreamWriter sw = new(cs);
            sw.Write(plainText);
            sw.Close();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }
        public string Decrypt(string cipherText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encKey);
            aes.IV = Encoding.UTF8.GetBytes(_encIV);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream ms = new(Convert.FromBase64String(cipherText));
            using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
            using StreamReader sr = new(cs);
            return sr.ReadToEnd();
        }
    }
}
