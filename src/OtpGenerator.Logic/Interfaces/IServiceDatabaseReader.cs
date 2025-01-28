using OtpGenerator.Entities.Generator;

namespace OtpGenerator.Logic.Interfaces
{
    public interface IServiceDatabaseReader
    {
        IList<ServiceDefinition> Read(string path, byte[] userSecret);
    }
}