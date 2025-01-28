using OtpGenerator.Entities.Generator;

namespace OtpGenerator.Logic.Interfaces
{
    public interface IServiceDatabaseWriter
    {
        void Write(IEnumerable<ServiceDefinition> services, string path, byte[] userSecret);
    }
}