using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;
using Moq;
using OtpGenerator.Tests.Mocks;
using OtpGenerator.Logic.Storage;
using OtpGenerator.Logic.Generator;
using OtpGenerator.Entities.Generator;
using OtpGenerator.CommandLine.Interfaces;
using OtpGenerator.CommandLine.Logic;
using OtpGenerator.CommandLine.Entities;
using OtpGenerator.DataExchange.Export;
using OtpGenerator.DataExchange.Entities;
using OtpGenerator.DataExchange.Import;

namespace OtpGenerator.Tests.CommandLineTests
{
    [TestClass]
    public class CommandLineDispatcherTest
    {
        private readonly MockOperationResultHandler _handler = new();
        private readonly IContextProvider _contextProvider = new ContextProvider();
        private IServiceDatabaseManager _manager;
        private ICommandLineDispatcher _dispatcher;
        private readonly ICommandLineParser _parser = new CommandLineParser();
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
   
            var secretProvider = new Mock<IUserSecretProvider>();
            secretProvider.Setup(x => x.GetSecret()).Returns(DataGenerator.RandomUserSecret());

            _manager = new ServiceDatabaseManager(secretProvider.Object, reader, writer, _dataFilePath);
         
            var generator = new CodeGenerator();
            var importer = new ServiceDefinitionImporter(_manager, ExportableServiceDefinition.CsvRecordPattern);
            var exporter = new ServiceDefinitionExporter();
            var timer = new Mock<IGeneratorTimer>();
            var liveGenerator = new LiveGenerator(generator, timer.Object);
            _dispatcher = new CommandLineDispatcher(generator, liveGenerator, _manager, null, _contextProvider, importer, exporter);

            _parser.Add(CommandLineOptionType.Help, true, "--help", "-h", "Show command line help", "", 0, 0);
            _parser.Add(CommandLineOptionType.Add, true, "--add", "-a", "Add a new service definition", "name account secret", 3, 3);
            _parser.Add(CommandLineOptionType.Delete, true, "--delete", "-d", "Delete an existing service definition", "id | name account", 2, 2);
            _parser.Add(CommandLineOptionType.List, true, "--list", "-l", "List the current service definitions", "[[id | name | name account]]", 0, 2);
            _parser.Add(CommandLineOptionType.Generate, true, "--generate", "-g", "Generate a TOTP for an account", "[[id | name | name account]]", 0, 2);
            _parser.Add(CommandLineOptionType.LiveView, true, "--live", "-li", "Show a live view for a service", "[[id | name | name account]]", 0, 2);
            _parser.Add(CommandLineOptionType.Import, true, "--import", "-i", "Import service definitions from a CSV file", "filepath", 1, 1);
            _parser.Add(CommandLineOptionType.Export, true, "--export", "-e", "Export service definitions to a CSV file", "filepath", 1, 1);

            _parser.Add(CommandLineOptionType.Verbose, false, "--verbose", "-v", "Enable verbose output", "", 0, 0);
            _parser.Add(CommandLineOptionType.ShowSecrets, false, "--show-secrets", "-sse", "Enable display of secrets", "", 0, 0);
            _parser.Add(CommandLineOptionType.Dummy, false, "--dummy", "-du", "Run in dummy mode, with sumulated service data", "", 0, 0);
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
        public void AddCommandTest()
        {
            var name = DataGenerator.RandomServiceName();
            var account = DataGenerator.RandomAccountName();
            var secret = DataGenerator.RandomServiceSecret();

            _parser.Parse(["--add", name, account, secret]);
            _dispatcher.RegisterAddCommandHandler(_handler);
            _dispatcher.Dispatch(_parser);

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

        [TestMethod]
        public void DeleteCommandTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _parser.Parse(["--delete", definition.Name, definition.Account]);
            _dispatcher.RegisterDeleteCommandHandler(_handler);
            _dispatcher.Dispatch(_parser);

            Assert.AreEqual(0, _manager.Services.Count);
            Assert.AreEqual(1, _handler.MessageResults.Count);
            Assert.IsFalse(string.IsNullOrEmpty(_handler.MessageResults[0]));
        }

        [TestMethod]
        public void ListTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _parser.Parse(["--list"]);
            _dispatcher.RegisterListCommandHandler(_handler);
            _dispatcher.Dispatch(_parser);

            var services = _handler.ObjectResults[0] as IList<ServiceDefinition>;
            Assert.IsNotNull(services);
            Assert.AreEqual(1, services.Count);
            Assert.IsNotNull(services[0]);
            Assert.AreEqual(definition.Name, services[0].Name);
            Assert.AreEqual(definition.Account, services[0].Account);
            Assert.AreEqual(definition.Secret, services[0].Secret);
        }

        [TestMethod]
        public void GenerateTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _parser.Parse(["--generate", definition.Name, definition.Account]);
            _dispatcher.RegisterGenerateCommandHandler(_handler);
            _dispatcher.Dispatch(_parser);

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
        public void ImportTest()
        {
            var name = DataGenerator.RandomServiceName();
            var account = DataGenerator.RandomAccountName();
            var secret = DataGenerator.RandomServiceSecret();
            var record = $@"""{name}"",""{account}"",""{secret}""";
            File.WriteAllLines(_csvFilePath, ["", record]);

            Assert.AreEqual(0, _manager.Services.Count);

            _parser.Parse(["--import", _csvFilePath]);
            _dispatcher.RegisterImportCommandHandler(_handler);
            _dispatcher.Dispatch(_parser);

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

        [TestMethod]
        public void ExportTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _parser.Parse(["--export", _csvFilePath]);
            _dispatcher.RegisterExportCommandHandler(_handler);
            _dispatcher.Dispatch(_parser);

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

        [TestMethod]
        public void LiveViewTest()
        {
            var definition = DataGenerator.RandomServiceDefinition();
            _manager.AddService(definition);
            _manager.Save();

            _parser.Parse(["--live"]);
            _dispatcher.RegisterLiveViewCommandHandler(_handler);
            _dispatcher.Dispatch(_parser);

            var liveGenerator = _handler.ObjectResults[0] as ILiveGenerator;
            Assert.IsNotNull(liveGenerator);
            Assert.IsNotNull(liveGenerator.Services);
            Assert.AreEqual(1, liveGenerator.Services.Count);
            Assert.AreEqual(definition.Name, liveGenerator.Services[0].Name);
            Assert.AreEqual(definition.Account, liveGenerator.Services[0].Account);
            Assert.AreEqual(definition.Secret, liveGenerator.Services[0].Secret);
        }

        [TestMethod]
        public void SetVerboseTest()
        {
            Assert.IsFalse(_contextProvider.Verbose);

            _parser.Parse(["--verbose"]);
            _dispatcher.RegisterVerboseContextOptionHandler(_handler);
            _dispatcher.Dispatch(_parser);

            Assert.IsTrue(_contextProvider.Verbose);
            Assert.IsNotNull(_handler.ObjectResults);
        }

        [TestMethod]
        public void SetShowSecretsTest()
        {
            Assert.IsFalse(_contextProvider.ShowSecrets);

            _parser.Parse(["--show-secrets"]);
            _dispatcher.RegisterShowSecretsContextOptionHandler(_handler);
            _dispatcher.Dispatch(_parser);

            Assert.IsTrue(_contextProvider.ShowSecrets);
            Assert.IsNotNull(_handler.ObjectResults);
        }

        [TestMethod]
        public void SetDummyTest()
        {
            Assert.IsFalse(_contextProvider.Dummy);

            _parser.Parse(["--dummy"]);
            _dispatcher.RegisterDummyContextOptionHandler(_handler);
            _dispatcher.Dispatch(_parser);

            Assert.IsTrue(_contextProvider.Dummy);
            Assert.IsNotNull(_handler.ObjectResults);
        }
    }
}