using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Storage;
using OtpGenerator.Entities.Exceptions;
using Moq;

namespace OtpGenerator.Tests.StorageTests
{
    [TestClass]
    public class ServiceDatabaseManagerTest
    {
        private IServiceDatabaseReader _reader;
        private IServiceDatabaseWriter _writer;
        private IServiceDatabaseManager _manager;
        private string _dataFilePath;
        private byte[] _userSecret;
        private readonly ServiceDefinition _definition = DataGenerator.RandomServiceDefinition();

        [TestInitialize]
        public void Initialise()
        {
            _dataFilePath = DataGenerator.TemporaryDataFilePath();
            _userSecret = DataGenerator.RandomUserSecret();

            var settings = DataGenerator.MockApplicationSettings();
            var keyGenerator = new EncryptionKeyGenerator(settings);
            var decryptor = new DataDecryptor(keyGenerator);
            var encryptor = new DataEncryptor(keyGenerator);

            var secretProvider = new Mock<IUserSecretProvider>();
            secretProvider.Setup(x => x.GetSecret()).Returns(_userSecret);

            _reader = new ServiceDatabaseReader(decryptor);
            _writer = new ServiceDatabaseWriter(encryptor);
            _manager = new ServiceDatabaseManager(secretProvider.Object, _reader, _writer, _dataFilePath);
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
        public void LoadTest()
        {
            _writer.Write([_definition], _dataFilePath, _userSecret);
            _manager.Load();

            Assert.AreEqual(1, _manager.Services.Count);
            Assert.IsTrue(_manager.Services[0].Id > 0);
            Assert.AreEqual(_definition.Name, _manager.Services[0].Name);
            Assert.AreEqual(_definition.Account, _manager.Services[0].Account);
            Assert.AreEqual(_definition.Secret, _manager.Services[0].Secret);
        }

        [TestMethod]
        public void LoadWithNoDataFilePresentTest()
        {
            _manager.Load();
            Assert.AreEqual(0, _manager.Services.Count);
        }

        [TestMethod]
        public void SaveTest()
        {
            _manager.AddService(_definition);
            _manager.Save();
            _manager.Clear();
            Assert.AreEqual(0, _manager.Services.Count);

            _manager.Load();

            Assert.AreEqual(1, _manager.Services.Count);
            Assert.AreEqual(_definition.Name,_manager.Services[0].Name);
            Assert.AreEqual(_definition.Account, _manager.Services[0].Account);
            Assert.AreEqual(_definition.Secret, _manager.Services[0].Secret);
        }


        [TestMethod]
        public void ClearTest()
        {
            _writer.Write([_definition], _dataFilePath, _userSecret);
            _manager.Load();

            Assert.AreEqual(1, _manager.Services.Count);

            _manager.Clear();

            Assert.AreEqual(0, _manager.Services.Count);
        }

        [TestMethod]
        public void AddTest()
        {
            Assert.AreEqual(0, _manager.Services.Count);

            _manager.AddService(_definition);

            Assert.AreEqual(1, _manager.Services.Count);
            Assert.AreEqual(_definition.Name, _manager.Services[0].Name);
            Assert.AreEqual(_definition.Account, _manager.Services[0].Account);
            Assert.AreEqual(_definition.Secret, _manager.Services[0].Secret);
        }

        [TestMethod]
        public void AddByNameAndSecretTest()
        {
            Assert.AreEqual(0, _manager.Services.Count);

            _manager.AddService(_definition.Name, _definition.Account, _definition.Secret);

            Assert.AreEqual(1, _manager.Services.Count);
            Assert.AreEqual(_definition.Name, _manager.Services[0].Name);
            Assert.AreEqual(_definition.Account, _manager.Services[0].Account);
            Assert.AreEqual(_definition.Secret, _manager.Services[0].Secret);
        }

        [TestMethod]
        public void AddDifferentAccountsForTheSameServiceTest()
        {
            Assert.AreEqual(0, _manager.Services.Count);

            var secondAccount = DataGenerator.RandomAccountName();
            _manager.AddService(_definition.Name, _definition.Account, _definition.Secret);
            _manager.AddService(_definition.Name, secondAccount, _definition.Secret);

            Assert.AreEqual(2, _manager.Services.Count);

            var first = _manager.Services.First(x => x.Account == _definition.Account);
            Assert.AreEqual(_definition.Name, first.Name);
            Assert.AreEqual(_definition.Account, first.Account);
            Assert.AreEqual(_definition.Secret, first.Secret);

            var second = _manager.Services.First(x => x.Account == secondAccount);
            Assert.AreEqual(_definition.Name, second.Name);
            Assert.AreEqual(secondAccount, second.Account);
            Assert.AreEqual(_definition.Secret, second.Secret);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateServiceException))]
        public void CannotAddDuplicateServiceAndAccountTest()
        {
            _manager.AddService(_definition.Name, _definition.Account, _definition.Secret);
            _manager.AddService(_definition.Name, _definition.Account, _definition.Secret);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidServiceDefinitionPropertyException))]
        public void CannotAddDefinitionWithEmptyNameTest()
            => _manager.AddService("", _definition.Account, _definition.Secret);

        [TestMethod]
        [ExpectedException(typeof(InvalidServiceDefinitionPropertyException))]
        public void CannotAddDefinitionWithNullNameTest()
            => _manager.AddService(null, _definition.Account, _definition.Secret);

        [TestMethod]
        [ExpectedException(typeof(InvalidServiceDefinitionPropertyException))]
        public void CannotAddDefinitionWithEmptyAccountTest()
            => _manager.AddService(_definition.Name, "", _definition.Secret);

        [TestMethod]
        [ExpectedException(typeof(InvalidServiceDefinitionPropertyException))]
        public void CannotAddDefinitionWithNullAccountTest()
            => _manager.AddService(_definition.Name, null, _definition.Secret);

        [TestMethod]
        [ExpectedException(typeof(InvalidServiceDefinitionPropertyException))]
        public void CannotAddDefinitionWithEmptySecretTest()
            => _manager.AddService(_definition.Name, _definition.Account, "");

        [TestMethod]
        [ExpectedException(typeof(InvalidServiceDefinitionPropertyException))]
        public void CannotAddDefinitionWithNullSecretTest()
            => _manager.AddService(_definition.Name, _definition.Account, null);

        [TestMethod]
        public void GetByNameTest()
        {
            _manager.AddService(_definition.Name, _definition.Account, _definition.Secret);
            var definitions = _manager.GetServices(_definition.Name);

            Assert.AreEqual(1, definitions.Count);
            Assert.AreEqual(_definition.Name, definitions[0].Name);
            Assert.AreEqual(_definition.Account, definitions[0].Account);
            Assert.AreEqual(_definition.Secret, definitions[0].Secret);
        }

        [TestMethod]
        public void GetByNameAndAccountTest()
        {
            _manager.AddService(_definition.Name, _definition.Account, _definition.Secret);
            var definition = _manager.GetService(_definition.Name, _definition.Account);

            Assert.AreEqual(_definition.Name, definition.Name);
            Assert.AreEqual(_definition.Account, definition.Account);
            Assert.AreEqual(_definition.Secret, definition.Secret);
        }

        [TestMethod]
        public void GetByIdTest()
        {
            _manager.AddService(_definition.Name, _definition.Account, _definition.Secret);
            var definition = _manager.GetService(1);

            Assert.AreEqual(_definition.Name, definition.Name);
            Assert.AreEqual(_definition.Account, definition.Account);
            Assert.AreEqual(_definition.Secret, definition.Secret);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceNotFoundException))]
        public void GetMissingByNameTest()
            => _manager.GetServices(_definition.Name);

        [TestMethod]
        [ExpectedException(typeof(ServiceNotFoundException))]
        public void GetMissingByNameAndAccountTest()
            => _manager.GetService(_definition.Name, _definition.Account);


        [TestMethod]
        [ExpectedException(typeof(ServiceNotFoundException))]
        public void GetMissingByIdTest()
            => _manager.GetService(0);

        [TestMethod]
        public void DeleteTest()
        {
            _manager.AddService(_definition.Name, _definition.Account, _definition.Secret);

            Assert.AreEqual(1, _manager.Services.Count);

            _manager.DeleteService(_definition.Name.ToLower(), _definition.Account.ToLower());

            Assert.AreEqual(0, _manager.Services.Count);
        }
    }
}