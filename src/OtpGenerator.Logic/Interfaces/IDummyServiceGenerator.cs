using OtpGenerator.Entities.Generator;

namespace OtpGenerator.Logic.Interfaces
{
    public interface IDummyServiceGenerator
    {
        IList<ServiceDefinition> GenerateDefinitions(string namePrefix, string account, int numberOfDefinitions);
    }
}