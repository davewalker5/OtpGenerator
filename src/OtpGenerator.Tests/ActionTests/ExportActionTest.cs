using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.Logic.Storage;
using OtpGenerator.CommandLine.Actions;
using OtpGenerator.CommandLine.Interfaces;
using OtpGenerator.Logic.Generator;
using OtpGenerator.DataExchange.Export;
using OtpGenerator.DataExchange.Entities;
using Moq;

namespace OtpGenerator.Tests.ActionTests
{
    [TestClass]
    public class ExportActionTest
    {
        private readonly MockOperationResultHandler _handler = new();
        private readonly IContextProvider _contextProvider = new ContextProvider();
        private IServiceDatabaseManager _manager;
        private IAction _action;
        private string _dataFilePath;
        private string _csvFilePath;

        [TestInitialize]
        public void Initialise()
        {
            _dataFilePath = DataGenerator.TemporaryDataFilePath();
            _csvFilePath = DataGenerator.TemporaryExportFilePath();

            var settings = DataGenerator.MockApplicationSettings();
            var keyGenerator = new EncryptionKeyGenerator(settings);
            var decryptor = new DataDecryptor(keyGenerator);
            var encryptor = new DataEncryptor(keyGenerator);
            var reader = new ServiceDatabaseReader(decryptor);
            var writer = new ServiceDatabaseWriter(encryptor);
            var exporter = new ServiceDefinitionExporter();
   
            var secretProvider = new Mock<IUserSecretProvider>();
            secretProvider.Setup(x => x.GetSecret()).Returns(DataGenerator.RandomUserSecret());

            _manager = new ServiceDatabaseManager(secretProvider.Object, reader, writer, _dataFilePath);
            _action = new ExportAction(_contextProvider, _manager, null, exporter, _handler);
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
        public void VerboseExportTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _contextProvider.Verbose = true;
            _action.Execute([_csvFilePath]);

            var info = new FileInfo(_csvFilePath);
            Assert.AreEqual(info.FullName, _csvFilePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_csvFilePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableServiceDefinition.FromCsv(records[1]);
            Assert.AreEqual(definition.Name, exportable.Name);
            Assert.AreEqual(definition.Account, exportable.Account);
            Assert.AreEqual(definition.Secret, exportable.Secret);

            Assert.AreEqual(2, _handler.MessageResults.Count);
            Assert.IsFalse(string.IsNullOrEmpty(_handler.MessageResults[0]));
            Assert.IsFalse(string.IsNullOrEmpty(_handler.MessageResults[1]));
        }

        [TestMethod]
        public void QuietExportTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _contextProvider.Verbose = false;
            _action.Execute([_csvFilePath]);

            var info = new FileInfo(_csvFilePath);
            Assert.AreEqual(info.FullName, _csvFilePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_csvFilePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableServiceDefinition.FromCsv(records[1]);
            Assert.AreEqual(definition.Name, exportable.Name);
            Assert.AreEqual(definition.Account, exportable.Account);
            Assert.AreEqual(definition.Secret, exportable.Secret);

            Assert.AreEqual(1, _handler.MessageResults.Count);
            Assert.IsFalse(string.IsNullOrEmpty(_handler.MessageResults[0]));
        }
    }
}