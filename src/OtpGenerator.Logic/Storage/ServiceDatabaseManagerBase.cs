using System.Collections.ObjectModel;
using OtpGenerator.Entities.Exceptions;
using OtpGenerator.Entities.Generator;

namespace OtpGenerator.Logic.Storage
{
    public abstract class ServiceDatabaseManagerBase
    {
        protected IList<ServiceDefinition> _services = [];

        public ReadOnlyCollection<ServiceDefinition> Services { get { return _services.OrderBy(x => x.Name)
                                                                                      .ThenBy(x => x.Account)
                                                                                      .ToList()
                                                                                      .AsReadOnly(); } }

        /// <summary>
        /// Clear the current selection of services
        /// </summary>
        public void Clear()
            => _services.Clear();

        /// <summary>
        /// Add a service given its definition
        /// </summary>
        /// <param name="service"></param>
        /// <exception cref="DuplicateServiceException"></exception>
        public void AddService(ServiceDefinition service)
        {
            // Validate the service definition properties
            ValidateServiceProperty("Name", service.Name);
            ValidateServiceProperty("Account", service.Account);
            ValidateServiceProperty("Secret", service.Secret);

            // Check the service isn't already defined
            var existing = _services.FirstOrDefault(x =>
                                x.Name.Equals(service.Name, StringComparison.OrdinalIgnoreCase) &&
                                x.Account.Equals(service.Account, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                var message = $"Service named '{service.Name}' for account '{service.Account}' already exists";
                throw new DuplicateServiceException(message);
            }

            // Assign an ID to the service and add it to the collection
            service.Id = GetNextId();
            _services.Add(service);
        }

        /// <summary>
        /// Add a service given a name and secret
        /// </summary>
        /// <param name="name"></param>
        /// <param name="account"></param>
        /// <param name="secret"></param>
        public ServiceDefinition AddService(string name, string account, string secret)
        {
            var definition = new ServiceDefinition{ Name = name, Account = account, Secret = secret };
            AddService(definition);
            return definition;
        }

        /// <summary>
        /// Find a service by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceDefinition GetService(int id)
        {
            var service = _services.FirstOrDefault(x => x.Id == id);
            if (service == null)
            {
                var message = $"Service with ID '{id}' not found";
                throw new ServiceNotFoundException(message);
            }

            return service;
        }

        /// <summary>
        /// Find a service by name and account
        /// </summary>
        /// <param name="name"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public ServiceDefinition GetService(string name, string account)
        {
            var service = _services.FirstOrDefault(x =>
                                x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                x.Account.Equals(account, StringComparison.OrdinalIgnoreCase));
            if (service == null)
            {
                var message = $"Service named '{name}' for account '{account}' not found";
                throw new ServiceNotFoundException(message);
            }

            return service;
        }

        /// <summary>
        /// Find all service/account/secret combinations for a named service
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IList<ServiceDefinition> GetServices(string name)
        {
            var services = _services.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!services.Any())
            {
                var message = $"No services named '{name}' were found";
                throw new ServiceNotFoundException(message);
            }

            return services;
        }

        /// <summary>
        /// Delete a service given its ID
        /// </summary>
        /// <param name="id"></param>
        public void DeleteService(int id)
        {
            var service = GetService(id);
            _services.Remove(service);
        }

        /// <summary>
        /// Delete a named service for a named account
        /// </summary>
        /// <param name="name"></param>
        /// <param name="account"></param>
        public void DeleteService(string name, string account)
        {
            var service = GetService(name, account);
            _services.Remove(service);
        }

        /// <summary>
        /// Validate a named service definition property value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <exception cref="InvalidServiceDefinitionPropertyException"></exception>
        private void ValidateServiceProperty(string property, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                var message = $"The '{property}' property of a service definition cannot be empty";
                throw new InvalidServiceDefinitionPropertyException(message);
            }
        }

        /// <summary>
        /// Return the next service identifier
        /// </summary>
        /// <returns></returns>
        protected int GetNextId()
            => !_services.Any() ? 1 : _services.Max(x => x.Id) + 1;
    }
}