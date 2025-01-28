using OtpGenerator.Entities.Generator;

namespace OtpGenerator.Logic.Interfaces
{
    public interface ICodeGenerator
    {
        TimedOneTimePassword Generate(ServiceDefinition service);
    }
}