using OtpGenerator.Entities.Exceptions;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Cryptography
{
    public abstract class EncryptionBase
    {
        public const int AesKeyLength = 32;

        protected readonly IEncryptionKeyGenerator _generator = null;

        public EncryptionBase(IEncryptionKeyGenerator provider)
            => _generator = provider;

        /// <summary>
        /// Check that data supplied for encryption/decryption is valid
        /// </summary>
        /// <param name="data"></param>
        protected static void ValidateData(byte[] data)
            => ValidateByteArrayLength<InvalidDataForEncryptionDecryptionException>(data, "Data for encryption/decryption", 1);

        /// <summary>
        /// Check that a user secret is valid
        /// </summary>
        /// <param name="userSecret"></param>
        protected static void ValidateUserSecret(byte[] userSecret)
            => ValidateByteArrayLength<InvalidUserSecretException>(userSecret, "User secret", 1);

        /// <summary>
        /// Check that a salt is valid
        /// </summary>
        /// <param name="salt"></param>
        protected static void ValidateSalt(byte[] salt)
            => ValidateByteArrayLength<InvalidSaltException>(salt, "Salt", 1);

        /// <summary>
        /// Check the length of a byte array and throw an exception of the specified type if it's
        /// less than the specified minimum length
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="name"></param>
        /// <param name="minimum"></param>
        private static void ValidateByteArrayLength<T>(byte[] data, string name, int minimum) where T : Exception, new()
        {
            if ((data?.Length ?? 0) < minimum)
            {
                var message = $"{name} cannot be empty";
                var exception = Activator.CreateInstance(typeof(T), [message]) as T;
                throw exception;
            }    
        }
    }
}