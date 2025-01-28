using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidServiceDefinitionPropertyException : Exception
    {
        public InvalidServiceDefinitionPropertyException()
        {
        }

        public InvalidServiceDefinitionPropertyException(string message) : base(message)
        {
        }

        public InvalidServiceDefinitionPropertyException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
