using System.Collections.ObjectModel;
using OtpGenerator.Entities.Generator;

namespace OtpGenerator.Logic.Interfaces
{
    public interface ILiveGenerator
    {
        event EventHandler<TotpUpdatedEventArgs> TotpUpdated;
        ReadOnlyCollection<ServiceDefinition> Services { get; }
        void AddService(ServiceDefinition definition);
        void AddServices(IEnumerable<ServiceDefinition> definitions);
        void ClearServices();
        void Start();
        void Stop();
    }
}