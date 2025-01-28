using OtpGenerator.Tests.Mocks;
using OtpGenerator.Entities.Generator;
using OtpGenerator.DataExchange.Extensions;
using OtpGenerator.DataExchange.Entities;
using OtpGenerator.DataExchange.Export;

namespace OtpGenerator.Tests.DataExchangeTests
{
    [TestClass]
    public class ServiceDefinitionExporterTest
    {
        private readonly ServiceDefinition _definition = DataGenerator.RandomServiceDefinition();
        private string _csvFilePath;

        [TestInitialize]
        public void Intiialise()
        {
            _csvFilePath = DataGenerator.TemporaryExportFilePath();
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (File.Exists(_csvFilePath))
            {
                File.Delete(_csvFilePath);
            }
        }

        [TestMethod]
        public void ConvertSingleObjectToExportable()
        {
            var exportable = _definition.ToExportable();
            Assert.AreEqual(_definition.Name, exportable.Name);
            Assert.AreEqual(_definition.Account, exportable.Account);
            Assert.AreEqual(_definition.Secret, exportable.Secret);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<ServiceDefinition> definitions = [_definition];
            var exportable = definitions.ToExportable();
            Assert.AreEqual(_definition.Name, exportable.First().Name);
            Assert.AreEqual(_definition.Account, exportable.First().Account);
            Assert.AreEqual(_definition.Secret, exportable.First().Secret);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_definition.Name}"",""{_definition.Account}"",""{_definition.Secret}""";
            var exportable = ExportableServiceDefinition.FromCsv(record);
            Assert.AreEqual(_definition.Name, exportable.Name);
            Assert.AreEqual(_definition.Account, exportable.Account);
            Assert.AreEqual(_definition.Secret, exportable.Secret);
        }

        [TestMethod]
        public void ExportTest()
        {
            List<ServiceDefinition> definitions = [_definition];

            new ServiceDefinitionExporter().Export(definitions, _csvFilePath);

            var info = new FileInfo(_csvFilePath);
            Assert.AreEqual(info.FullName, _csvFilePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_csvFilePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableServiceDefinition.FromCsv(records[1]);
            Assert.AreEqual(_definition.Name, exportable.Name);
            Assert.AreEqual(_definition.Account, exportable.Account);
            Assert.AreEqual(_definition.Secret, exportable.Secret);
        }
    }
}