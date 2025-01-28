using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidDataForEncryptionDecryptionException : Exception
    {
        public InvalidDataForEncryptionDecryptionException()
        {
        }

        public InvalidDataForEncryptionDecryptionException(string message) : base(message)
        {
        }

        public InvalidDataForEncryptionDecryptionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
