using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.Logic.Storage;
using OtpGenerator.CommandLine.Actions;
using OtpGenerator.CommandLine.Interfaces;
using OtpGenerator.Logic.Generator;
using Moq;

namespace OtpGenerator.Tests.ActionTests
{
    [TestClass]
    public class DeleteActionTest
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
            _action = new DeleteAction(contextProvider, _manager, null, _handler);
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
        public void DeleteByIdTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            Assert.AreEqual(1, _manager.Services.Count);

            var serviceId = _manager.Services[0].Id;
            _action.Execute([serviceId.ToString()]);

            Assert.AreEqual(0, _manager.Services.Count);
            Assert.AreEqual(1, _handler.MessageResults.Count);
            Assert.IsFalse(string.IsNullOrEmpty(_handler.MessageResults[0]));
        }

        [TestMethod]
        public void DeleteByServiceAndAccountTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            Assert.AreEqual(1, _manager.Services.Count);

            _action.Execute([definition.Name, definition.Account]);

            Assert.AreEqual(0, _manager.Services.Count);
            Assert.AreEqual(1, _handler.MessageResults.Count);
            Assert.IsFalse(string.IsNullOrEmpty(_handler.MessageResults[0]));
        }
    }
}