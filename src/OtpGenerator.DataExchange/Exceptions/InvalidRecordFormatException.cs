using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.DataExchange.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidRecordFormatException : Exception
    {
        public InvalidRecordFormatException()
        {
        }

        public InvalidRecordFormatException(string message) : base(message)
        {
        }

        public InvalidRecordFormatException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
