using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.Logic.Storage;
using OtpGenerator.CommandLine.Actions;
using OtpGenerator.Entities.Generator;
using OtpGenerator.CommandLine.Interfaces;
using OtpGenerator.Logic.Generator;
using Moq;

namespace OtpGenerator.Tests.ActionTests
{
    [TestClass]
    public class ListActionTest
    {
        private readonly MockOperationResultHandler _handler = new();
        private IServiceDatabaseManager _manager;
        private IAction _action;
        private string _dataFilePath;

        [TestInitialize]
        public void Initialise()
        {
            _dataFilePath = DataGenerator.TemporaryDataFilePath();

            var contextProvider = new ContextProvider();
            var settings = DataGenerator.MockApplicationSettings();
            var keyGenerator = new EncryptionKeyGenerator(settings);
            var decryptor = new DataDecryptor(keyGenerator);
            var encryptor = new DataEncryptor(keyGenerator);
            var reader = new ServiceDatabaseReader(decryptor);
            var writer = new ServiceDatabaseWriter(encryptor);
   
            var secretProvider = new Mock<IUserSecretProvider>();
            secretProvider.Setup(x => x.GetSecret()).Returns(DataGenerator.RandomUserSecret());

            _manager = new ServiceDatabaseManager(secretProvider.Object, reader, writer, _dataFilePath);
            _action = new ListAction(contextProvider, _manager, null, _handler);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (File.Exists(_dataFilePath))
            {
                File.Delete(_dataFilePath);
            }
        }

        [TestMethod]
        public void ListByServiceIdTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            var serviceId = _manager.Services[0].Id;
            _action.Execute([serviceId.ToString()]);

            var services = _handler.ObjectResults[0] as IList<ServiceDefinition>;
            Assert.AreEqual(1, services.Count);
            Assert.AreEqual(definition.Name, services[0].Name);
            Assert.AreEqual(definition.Account, services[0].Account);
            Assert.AreEqual(definition.Secret, services[0].Secret);
        }

        [TestMethod]
        public void ListByServiceNameTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _action.Execute([definition.Name]);

            var services = _handler.ObjectResults[0] as IList<ServiceDefinition>;
            Assert.AreEqual(1, services.Count);
            Assert.AreEqual(definition.Name, services[0].Name);
            Assert.AreEqual(definition.Account, services[0].Account);
            Assert.AreEqual(definition.Secret, services[0].Secret);
        }

        [TestMethod]
        public void ListByServiceNameAndAccountTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _action.Execute([definition.Name, definition.Account]);

            var services = _handler.ObjectResults[0] as IList<ServiceDefinition>;
            Assert.AreEqual(1, services.Count);
            Assert.AreEqual(definition.Name, services[0].Name);
            Assert.AreEqual(definition.Account, services[0].Account);
            Assert.AreEqual(definition.Secret, services[0].Secret);
        }

        [TestMethod]
        public void ListAllTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _action.Execute([]);

            var services = _handler.ObjectResults[0] as IList<ServiceDefinition>;
            Assert.AreEqual(1, services.Count);
            Assert.AreEqual(definition.Name, services[0].Name);
            Assert.AreEqual(definition.Account, services[0].Account);
            Assert.AreEqual(definition.Secret, services[0].Secret);
        }
    }
}