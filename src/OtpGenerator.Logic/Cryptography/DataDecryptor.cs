using System.Security.Cryptography;
using System.Text;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Cryptography
{
    public class DataDecryptor : EncryptionBase, IDataDecryptor
    {
        public DataDecryptor(IEncryptionKeyGenerator provider) : base(provider)
        {
        }

        /// <summary>
        /// Decrypt a byte array
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userSecret"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] data, byte[] userSecret, byte[] salt)
        {
            var decrypted = data;

            // Validate the input data and the encryption key
            ValidateData(data);
            ValidateUserSecret(userSecret);
            ValidateSalt(salt);

            using (Aes aes = Aes.Create())
            {
                // Configure AES
                aes.Key = _generator.GenerateKey(userSecret, salt);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Extract IV from the beginning of the encrypted data. It's always 16 bytes long for AES
                byte[] iv = new byte[16];
                Array.Copy(data, 0, iv, 0, iv.Length);

                // Set up a memory stream contaiuning the encrypted data
                using (MemoryStream ms = new(data, iv.Length, data.Length - iv.Length))
                {
                    // Set up the decryptor to read from the stream
                    using (CryptoStream cs = new(ms, aes.CreateDecryptor(aes.Key, iv), CryptoStreamMode.Read))
                    {
                        // Read the decrypted data
                        using (StreamReader sr = new(cs))
                        {
                            decrypted = Encoding.UTF8.GetBytes(sr.ReadToEnd());
                        }
                    }
                }
            }

            return decrypted;
        }

    }
}