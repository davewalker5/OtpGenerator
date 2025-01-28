using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidSaltException : Exception
    {
        public InvalidSaltException()
        {
        }

        public InvalidSaltException(string message) : base(message)
        {
        }

        public InvalidSaltException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
