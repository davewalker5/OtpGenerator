using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Storage;

namespace OtpGenerator.Tests.StorageTests
{
    [TestClass]
    public class ServiceReaderWriterTest
    {
        private IServiceDatabaseReader _reader;
        private IServiceDatabaseWriter _writer;
        private readonly ServiceDefinition _definition = DataGenerator.RandomServiceDefinition();

        private string _dataFilePath;

        [TestInitialize]
        public void Initialise()
        {
            _dataFilePath = DataGenerator.TemporaryDataFilePath();

            var settings = DataGenerator.MockApplicationSettings();
            var keyGenerator = new EncryptionKeyGenerator(settings);
            var decryptor = new DataDecryptor(keyGenerator);
            var encryptor = new DataEncryptor(keyGenerator);

            _reader = new ServiceDatabaseReader(decryptor);
            _writer = new ServiceDatabaseWriter(encryptor);
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
        public void ReadAndWriteTest()
        {
            var userSecret = DataGenerator.RandomUserSecret();
            _writer.Write([_definition], _dataFilePath, userSecret);
            var loaded = _reader.Read(_dataFilePath, userSecret);

            Assert.IsNotNull(loaded);
            Assert.AreEqual(1, loaded.Count);
            Assert.AreEqual(_definition.Name, loaded[0].Name);
            Assert.AreEqual(_definition.Secret, loaded[0].Secret);
        }
    }
}