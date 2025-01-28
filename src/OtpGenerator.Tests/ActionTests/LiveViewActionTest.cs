using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;
using Moq;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.Logic.Storage;
using OtpGenerator.Logic.Generator;
using OtpGenerator.CommandLine.Actions;
using OtpGenerator.CommandLine.Interfaces;

namespace OtpGenerator.Tests.ActionTests
{
    [TestClass]
    public class LiveViewActionTest
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
            var codeGenerator = new CodeGenerator();
            var timer = new Mock<IGeneratorTimer>();
            var liveGenerator = new LiveGenerator(codeGenerator, timer.Object);
   
            var secretProvider = new Mock<IUserSecretProvider>();
            secretProvider.Setup(x => x.GetSecret()).Returns(DataGenerator.RandomUserSecret());

            _manager = new ServiceDatabaseManager(secretProvider.Object, reader, writer, _dataFilePath);
            _action = new LiveViewAction(contextProvider, liveGenerator, _manager, null, _handler);
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
        public void LiveViewByServiceIdTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            Assert.AreEqual(1, _manager.Services.Count);

            var serviceId = _manager.Services[0].Id;
            _action.Execute([serviceId.ToString()]);

            var liveGenerator = _handler.ObjectResults[0] as ILiveGenerator;
            Assert.IsNotNull(liveGenerator);
            Assert.IsNotNull(liveGenerator.Services);
            Assert.AreEqual(1, liveGenerator.Services.Count);
            Assert.AreEqual(definition.Name, liveGenerator.Services[0].Name);
            Assert.AreEqual(definition.Account, liveGenerator.Services[0].Account);
            Assert.AreEqual(definition.Secret, liveGenerator.Services[0].Secret);
        }

        [TestMethod]
        public void LiveViewByServiceNameTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            Assert.AreEqual(1, _manager.Services.Count);

            _action.Execute([definition.Name]);

            var liveGenerator = _handler.ObjectResults[0] as ILiveGenerator;
            Assert.IsNotNull(liveGenerator);
            Assert.IsNotNull(liveGenerator.Services);
            Assert.AreEqual(1, liveGenerator.Services.Count);
            Assert.AreEqual(definition.Name, liveGenerator.Services[0].Name);
            Assert.AreEqual(definition.Account, liveGenerator.Services[0].Account);
            Assert.AreEqual(definition.Secret, liveGenerator.Services[0].Secret);
        }

        [TestMethod]
        public void LiveViewByServiceNameAndAccountTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            Assert.AreEqual(1, _manager.Services.Count);

            _action.Execute([definition.Name, definition.Account]);

            var liveGenerator = _handler.ObjectResults[0] as ILiveGenerator;
            Assert.IsNotNull(liveGenerator);
            Assert.IsNotNull(liveGenerator.Services);
            Assert.AreEqual(1, liveGenerator.Services.Count);
            Assert.AreEqual(definition.Name, liveGenerator.Services[0].Name);
            Assert.AreEqual(definition.Account, liveGenerator.Services[0].Account);
            Assert.AreEqual(definition.Secret, liveGenerator.Services[0].Secret);
        }

        [TestMethod]
        public void LiveViewAllTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            Assert.AreEqual(1, _manager.Services.Count);

            var serviceId = _manager.Services[0].Id;
            _action.Execute([serviceId.ToString()]);

            var liveGenerator = _handler.ObjectResults[0] as ILiveGenerator;
            Assert.IsNotNull(liveGenerator);
            Assert.IsNotNull(liveGenerator.Services);
            Assert.AreEqual(1, liveGenerator.Services.Count);
            Assert.AreEqual(definition.Name, liveGenerator.Services[0].Name);
            Assert.AreEqual(definition.Account, liveGenerator.Services[0].Account);
            Assert.AreEqual(definition.Secret, liveGenerator.Services[0].Secret);
        }
    }
}