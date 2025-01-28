using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Storage
{
    public class ServiceDatabaseManager : ServiceDatabaseManagerBase, IServiceDatabaseManager
    {
        private readonly IServiceDatabaseReader _reader;
        private readonly IServiceDatabaseWriter _writer;
        private readonly IUserSecretProvider _secretProvider;
        private readonly string _databaseFilePath;
        private byte[] _userSecret = null;

        public ServiceDatabaseManager(
            IUserSecretProvider userSecretProvider,
            IServiceDatabaseReader reader,
            IServiceDatabaseWriter writer,
            string databaseFilePath)
        {
            _reader = reader;
            _writer = writer;
            _databaseFilePath = databaseFilePath;
            _secretProvider = userSecretProvider;
        }

        /// <summary>
        /// Load the service definitions
        /// </summary>
        public void Load()
        {
            try
            {
                PopulateUserSecret();
                _services = _reader.Read(_databaseFilePath, _userSecret);
                PopulateIdentifiers();
            }
            catch (FileNotFoundException)
            {
                _services = [];
            }
        }

        /// <summary>
        /// Save the current service definitions
        /// </summary>
        public void Save()
        {
            PopulateUserSecret();
            _writer.Write(_services, _databaseFilePath, _userSecret);
        }

        /// <summary>
        /// Popullate service identifiers for any services that don't have them
        /// </summary>
        private void PopulateIdentifiers()
        {
            // Iterate over all definitions for which the ID isn't set and add an ID
            var servicesWithoutId = _services.Where(x => x.Id == 0);
            foreach (var service in servicesWithoutId)
            {
                service.Id = GetNextId();
            }

            // If any were added, re-save the database
            if (servicesWithoutId.Any())
            {
                _writer.Write(_services, _databaseFilePath, _userSecret);
            }
        }

        /// <summary>
        /// If the secret hasn't been set, use the provider to set it
        /// </summary>
        private void PopulateUserSecret()
            => _userSecret ??= _secretProvider.GetSecret();
    }
}