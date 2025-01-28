using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Storage;
using OtpGenerator.Entities.Exceptions;
using OtpGenerator.Configuration.Interfaces;
using OtpGenerator.Logic.Generator;

namespace OtpGenerator.Tests.StorageTests
{
    [TestClass]
    public class DummyServiceDatabaseManagerTest
    {
        private readonly IOtpGeneratorApplicationSettings _settings = DataGenerator.MockApplicationSettings();
        private readonly ServiceDefinition _definition = DataGenerator.RandomServiceDefinition();
        private IServiceDatabaseManager _manager;

        [TestInitialize]
        public void Initialise()
        {
            var settings = DataGenerator.MockApplicationSettings();
            var generator = new DummyServiceGenerator();
            _manager = new DummyServiceDatabaseManager(_settings, generator);
        }

        [TestMethod]
        public void LoadTest()
        {
            _manager.Load();

            Assert.AreEqual(_settings.NumberOfDummyServices, _manager.Services.Count);
            foreach (var definition in _manager.Services)
            {
                Assert.IsTrue(definition.Id > 0);
                Assert.IsTrue(definition.Name.Contains(_settings.DummyServiceName));
                Assert.IsTrue(definition.Account.Contains(_settings.DummyServiceAccount));
                Assert.IsFalse(string.IsNullOrEmpty(definition.Secret));
            }
        }

        [TestMethod]
        public void ClearTest()
        {
            _manager.Load();

            Assert.AreEqual(_settings.NumberOfDummyServices, _manager.Services.Count);

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