using System.Diagnostics.CodeAnalysis;
using OtpGenerator.Configuration.Interfaces;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Storage
{
    public class DummyServiceDatabaseManager : ServiceDatabaseManagerBase, IServiceDatabaseManager
    {
        private readonly IOtpGeneratorApplicationSettings _settings;
        private readonly IDummyServiceGenerator _generator;

        public DummyServiceDatabaseManager(IOtpGeneratorApplicationSettings settings, IDummyServiceGenerator generator)
        {
            _settings = settings;
            _generator = generator;
        }

        /// <summary>
        /// Load the service definitions
        /// </summary>
        public void Load()
            => _services = _generator.GenerateDefinitions(_settings.DummyServiceName, _settings.DummyServiceAccount, _settings.NumberOfDummyServices);

        /// <summary>
        /// Save the current service definitions
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Save()
        {
        }
    }
}