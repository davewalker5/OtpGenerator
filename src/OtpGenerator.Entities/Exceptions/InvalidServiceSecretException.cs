using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidServiceSecretException : Exception
    {
        public InvalidServiceSecretException()
        {
        }

        public InvalidServiceSecretException(string message) : base(message)
        {
        }

        public InvalidServiceSecretException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
