using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class DuplicateServiceException : Exception
    {
        public DuplicateServiceException()
        {
        }

        public DuplicateServiceException(string message) : base(message)
        {
        }

        public DuplicateServiceException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
