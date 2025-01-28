using OtpGenerator.Tests.Mocks;
using OtpGenerator.DataExchange.Entities;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Storage;
using OtpGenerator.DataExchange.Interfaces;
using OtpGenerator.DataExchange.Import;
using OtpGenerator.DataExchange.Exceptions;
using Moq;

namespace OtpGenerator.Tests.DataExchangeTests
{
    [TestClass]
    public class ServiceDefinitionImporterTest
    {
        private IServiceDatabaseManager _manager;
        private IServiceDefinitionImporter _importer;
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
            _importer = new ServiceDefinitionImporter(_manager, ExportableServiceDefinition.CsvRecordPattern);
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
        public void ImportTest()
        {
            var name = DataGenerator.RandomServiceName();
            var account = DataGenerator.RandomAccountName();
            var secret = DataGenerator.RandomServiceSecret();
            var record = $@"""{name}"",""{account}"",""{secret}""";
            File.WriteAllLines(_csvFilePath, ["", record]);

            Assert.AreEqual(0, _manager.Services.Count);

            _importer.Import(_csvFilePath);

            Assert.AreEqual(1, _manager.Services.Count);
            Assert.AreEqual(name, _manager.Services.First().Name);
            Assert.AreEqual(account, _manager.Services.First().Account);
            Assert.AreEqual(secret, _manager.Services.First().Secret);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public void InvalidRecordFormatTest()
        {
            File.WriteAllLines(_csvFilePath, ["", "Invalid record format"]);
            _importer.Import(_csvFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public void InvalidServiceNameTest()
        {
            var account = DataGenerator.RandomAccountName();
            var secret = DataGenerator.RandomServiceSecret();
            var record = $@""""",""{account}"",""{secret}""";
            File.WriteAllLines(_csvFilePath, ["", record]);

            _importer.Import(_csvFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public void InvalidAccountNameTest()
        {
            var name = DataGenerator.RandomServiceName();
            var secret = DataGenerator.RandomServiceSecret();
            var record = $@"""{name}"","""",""{secret}""";
            File.WriteAllLines(_csvFilePath, ["", record]);

            _importer.Import(_csvFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public void InvalidSecretTest()
        {
            var name = DataGenerator.RandomServiceName();
            var account = DataGenerator.RandomAccountName();
            var record = $@"""{name}"",""{account}"",""""";
            File.WriteAllLines(_csvFilePath, ["", record]);

            _importer.Import(_csvFilePath);
        }
    }
}