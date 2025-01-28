using OtpGenerator.Tests.Mocks;
using OtpGenerator.DataExchange.Entities;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Storage;
using OtpGenerator.DataExchange.Import;
using OtpGenerator.CommandLine.Actions;
using OtpGenerator.Logic.Generator;
using OtpGenerator.CommandLine.Interfaces;
using Moq;

namespace OtpGenerator.Tests.DataExchangeTests
{
    [TestClass]
    public class ImportActionTest
    {
        private readonly MockOperationResultHandler _handler = new();
        private readonly IContextProvider _contextProvider = new ContextProvider();
        private IServiceDatabaseManager _manager;
        private IAction _action;
        private string _dataFilePath;
        private string _csvFilePath;

        [TestInitialize]
        public void Intiialise()
        {
            _dataFilePath = DataGenerator.TemporaryDataFilePath();
            _csvFilePath = DataGenerator.TemporaryExportFilePath();

            var settings = DataGenerator.MockApplicationSettings();
            var keyGenerator = new EncryptionKeyGenerator(settings);
            var decryptor = new DataDecryptor(keyGenerator);
            var encryptor = new DataEncryptor(keyGenerator);
            var reader = new ServiceDatabaseReader(decryptor);
            var writer = new ServiceDatabaseWriter(encryptor);
   
            var secretProvider = new Mock<IUserSecretProvider>();
            secretProvider.Setup(x => x.GetSecret()).Returns(DataGenerator.RandomUserSecret());

            _manager = new ServiceDatabaseManager(secretProvider.Object, reader, writer, _dataFilePath);
            var importer = new ServiceDefinitionImporter(_manager, ExportableServiceDefinition.CsvRecordPattern);
            _action = new ImportAction(_contextProvider, _manager, null, importer, _handler);
        }

        [TestCleanup]
        public void CleanUp()
        {
            foreach (var file in new List<string>() {_csvFilePath, _dataFilePath})
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        [TestMethod]
        public void VerboseImportTest()
        {
            var name = DataGenerator.RandomServiceName();
            var account = DataGenerator.RandomAccountName();
            var secret = DataGenerator.RandomServiceSecret();
            var record = $@"""{name}"",""{account}"",""{secret}""";
            File.WriteAllLines(_csvFilePath, ["", record]);

            Assert.AreEqual(0, _manager.Services.Count);

            _contextProvider.Verbose = true;
            _action.Execute([_csvFilePath]);

            var info = new FileInfo(_dataFilePath);
            Assert.AreEqual(info.FullName, _dataFilePath);
            Assert.IsTrue(info.Length > 0);

            Assert.AreEqual(1, _manager.Services.Count);
            Assert.AreEqual(name, _manager.Services.First().Name);
            Assert.AreEqual(account, _manager.Services.First().Account);
            Assert.AreEqual(secret, _manager.Services.First().Secret);

            Assert.AreEqual(2, _handler.MessageResults.Count);
            Assert.IsFalse(string.IsNullOrEmpty(_handler.MessageResults[0]));
            Assert.IsFalse(string.IsNullOrEmpty(_handler.MessageResults[1]));
        }

        [TestMethod]
        public void QuietImportTest()
        {
            var name = DataGenerator.RandomServiceName();
            var account = DataGenerator.RandomAccountName();
            var secret = DataGenerator.RandomServiceSecret();
            var record = $@"""{name}"",""{account}"",""{secret}""";
            File.WriteAllLines(_csvFilePath, ["", record]);

            Assert.AreEqual(0, _manager.Services.Count);

            _contextProvider.Verbose = false;
            _action.Execute([_csvFilePath]);

            var info = new FileInfo(_dataFilePath);
            Assert.AreEqual(info.FullName, _dataFilePath);
            Assert.IsTrue(info.Length > 0);

            Assert.AreEqual(1, _manager.Services.Count);
            Assert.AreEqual(name, _manager.Services.First().Name);
            Assert.AreEqual(account, _manager.Services.First().Account);
            Assert.AreEqual(secret, _manager.Services.First().Secret);

            Assert.AreEqual(1, _handler.MessageResults.Count);
            Assert.IsFalse(string.IsNullOrEmpty(_handler.MessageResults[0]));
        }
    }
}