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
    public class AddActionTest
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
         
            var generator = new CodeGenerator();
            _action = new AddAction(contextProvider, generator, _manager, null, _handler);
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
        public void AddTest()
        {
            var name = DataGenerator.RandomServiceName();
            var account = DataGenerator.RandomAccountName();
            var secret = DataGenerator.RandomServiceSecret();

            _action.Execute([name, account, secret]);

            Assert.AreEqual(1, _manager.Services.Count);
            Assert.AreEqual(name, _manager.Services[0].Name);
            Assert.AreEqual(account, _manager.Services[0].Account);
            Assert.AreEqual(secret, _manager.Services[0].Secret);

            var totps = _handler.ObjectResults[0] as IList<TimedOneTimePassword>;
            Assert.IsNotNull(totps);
            Assert.AreEqual(1, totps.Count);
            Assert.IsNotNull(totps[0]);
            Assert.AreEqual(name, totps[0].ServiceName);
            Assert.AreEqual(account, totps[0].Account);
            Assert.AreEqual(6, totps[0].Code.Length);
            Assert.IsTrue(totps[0].RemainingSeconds > 0);
        }
    }
}