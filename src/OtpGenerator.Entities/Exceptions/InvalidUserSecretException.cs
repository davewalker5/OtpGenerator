using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidUserSecretException : Exception
    {
        public InvalidUserSecretException()
        {
        }

        public InvalidUserSecretException(string message) : base(message)
        {
        }

        public InvalidUserSecretException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
