using System.Security.Cryptography;
using Konscious.Security.Cryptography;
using OtpGenerator.Configuration.Interfaces;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Cryptography
{
    public class EncryptionKeyGenerator : IEncryptionKeyGenerator
    {
        public const int SaltLength = 16;
        private readonly IOtpGeneratorApplicationSettings _settings;

        public EncryptionKeyGenerator(IOtpGeneratorApplicationSettings settings)
            => _settings = settings;

        /// <summary>
        /// Generate the encryption key from a user secret and salt
        /// </summary>
        /// <param name="userSecret"></param>
        public byte[] GenerateKey(byte[] userSecret, byte[] salt)
        {
            byte[] key;
            using (var argon2 = new Argon2id(userSecret))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = _settings.KeyGenerationParallelism;
                argon2.MemorySize = _settings.KeyGenerationMemory;
                argon2.Iterations = _settings.KeyGenerationIterations;

                // Generate the hash (i.e. the key)
                key = argon2.GetBytes(EncryptionBase.AesKeyLength);
            }

            return key;
        }

        /// <summary>
        /// Generate a salt
        /// </summary>
        /// <param name="userSecret"></param>
        /// <returns></returns>
        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[SaltLength];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }
    }
}