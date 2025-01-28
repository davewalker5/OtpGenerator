using System.Collections.ObjectModel;
using OtpGenerator.Entities.Generator;

namespace OtpGenerator.Logic.Interfaces
{
    public interface IServiceDatabaseManager
    {
        ReadOnlyCollection<ServiceDefinition> Services { get; }
        void Load();
        void Save();
        void Clear();
        ServiceDefinition GetService(int id);
        ServiceDefinition GetService(string name, string account);
        IList<ServiceDefinition> GetServices(string name);
        void AddService(ServiceDefinition service);
        ServiceDefinition AddService(string name, string account, string secret);
        void DeleteService(int id);
        void DeleteService(string name, string account);
    }
}