using System.Security.Cryptography;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Cryptography
{
    public class DataEncryptor : EncryptionBase, IDataEncryptor
    {
        public DataEncryptor(IEncryptionKeyGenerator provider) : base(provider)
        {
        }

        /// <summary>
        /// Encrypt a byte array using a user secret
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userSecret"></param>
        /// <returns></returns>
        public (byte[] encrypted, byte[] salt) Encrypt(byte[] data, byte[] userSecret)
        {
            var encrypted = data;

            // Validate the input data and the user secret
            ValidateData(data);
            ValidateUserSecret(userSecret);

            // Generate a salt
            var salt = _generator.GenerateSalt();

            using (Aes aes = Aes.Create())
            {
                // Configure AES
                aes.Key = _generator.GenerateKey(userSecret, salt);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Set a random IV
                aes.GenerateIV();
                byte[] iv = aes.IV;

                // Create a memory stream to hold the encrypted data
                using (MemoryStream output = new())
                {
                    // Write the random IV first
                    output.Write(iv, 0, iv.Length);

                    // Set up the encryptor to write to the stream
                    using (CryptoStream cs = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }

                    // Extract the encrypted result
                    encrypted = output.ToArray();
                }
            }

            // Return the encrypted data and the salt
            return (encrypted, salt);
        }
    }
}