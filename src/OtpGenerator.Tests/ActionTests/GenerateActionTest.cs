using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.Logic.Storage;
using OtpGenerator.Logic.Generator;
using OtpGenerator.CommandLine.Actions;
using OtpGenerator.Entities.Generator;
using OtpGenerator.CommandLine.Interfaces;
using Moq;

namespace OtpGenerator.Tests.ActionTests
{
    [TestClass]
    public class GenerateActionTest
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
   
            var secretProvider = new Mock<IUserSecretProvider>();
            secretProvider.Setup(x => x.GetSecret()).Returns(DataGenerator.RandomUserSecret());

            _manager = new ServiceDatabaseManager(secretProvider.Object, reader, writer, _dataFilePath);
            _action = new GenerateAction(contextProvider, codeGenerator, _manager, null, _handler);
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
        public void GenerateByIdTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            var serviceId = _manager.Services[0].Id;
            _action.Execute([serviceId.ToString()]);

            var totps = _handler.ObjectResults[0] as IList<TimedOneTimePassword>;
            Assert.IsNotNull(totps);
            Assert.AreEqual(1, totps.Count);
            Assert.IsNotNull(totps[0]);
            Assert.AreEqual(1, totps[0].ServiceId);
            Assert.AreEqual(definition.Name, totps[0].ServiceName);
            Assert.AreEqual(definition.Account, totps[0].Account);
            Assert.AreEqual(6, totps[0].Code.Length);
            Assert.IsTrue(totps[0].RemainingSeconds > 0);
        }

        [TestMethod]
        public void GenerateByNameTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _action.Execute([definition.Name]);

            var totps = _handler.ObjectResults[0] as IList<TimedOneTimePassword>;
            Assert.IsNotNull(totps);
            Assert.AreEqual(1, totps.Count);
            Assert.IsNotNull(totps[0]);
            Assert.AreEqual(definition.Name, totps[0].ServiceName);
            Assert.AreEqual(definition.Account, totps[0].Account);
            Assert.AreEqual(6, totps[0].Code.Length);
            Assert.IsTrue(totps[0].RemainingSeconds > 0);
        }

        [TestMethod]
        public void GenerateByNameAndAccountTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _action.Execute([definition.Name, definition.Account]);

            var totps = _handler.ObjectResults[0] as IList<TimedOneTimePassword>;
            Assert.IsNotNull(totps);
            Assert.AreEqual(1, totps.Count);
            Assert.IsNotNull(totps[0]);
            Assert.AreEqual(definition.Name, totps[0].ServiceName);
            Assert.AreEqual(definition.Account, totps[0].Account);
            Assert.AreEqual(6, totps[0].Code.Length);
            Assert.IsTrue(totps[0].RemainingSeconds > 0);
        }

        [TestMethod]
        public void GenerateAllTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _action.Execute([]);

            var totps = _handler.ObjectResults[0] as IList<TimedOneTimePassword>;
            Assert.IsNotNull(totps);
            Assert.AreEqual(1, totps.Count);
            Assert.IsNotNull(totps[0]);
            Assert.AreEqual(1, totps[0].ServiceId);
            Assert.AreEqual(definition.Name, totps[0].ServiceName);
            Assert.AreEqual(definition.Account, totps[0].Account);
            Assert.AreEqual(6, totps[0].Code.Length);
            Assert.IsTrue(totps[0].RemainingSeconds > 0);
        }
    }
}