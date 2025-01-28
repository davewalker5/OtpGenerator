using System.Diagnostics;
using System.Reflection;
using OtpGenerator.CommandLine.Entities;
using OtpGenerator.CommandLine.Logic;
using OtpGenerator.Configuration.Logic;
using OtpGenerator.DataExchange.Entities;
using OtpGenerator.DataExchange.Export;
using OtpGenerator.DataExchange.Import;
using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Generator;
using OtpGenerator.Logic.Storage;
using OtpGenerator.Terminal.ResultHandlers;

namespace OtpGenerator.Terminal
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            LiveGenerator liveGenerator = null;

            try
            {
                // Get the version number and write the application title
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
                Console.WriteLine($"\nTOTP Generator Console Application v{info.FileVersion}\n");

                // Read the application settings
                var settings = new OtpGeneratorConfigReader().Read("appsettings.json");

                // Configure database file management
                var passwordProvider = new CommandLinePasswordProvider();
                var keyProvider = new EncryptionKeyGenerator(settings);
                var reader = new ServiceDatabaseReader(new DataDecryptor(keyProvider));
                var writer = new ServiceDatabaseWriter(new DataEncryptor(keyProvider));

                // Configure the command line
                var parser = new CommandLineParser(new HelpTabulator());

                // Create and configure the command line dispatcher
                var generator = new CodeGenerator();
                var manager = new ServiceDatabaseManager(passwordProvider, reader, writer, settings.DatabasePath);
                var dummyServiceGenerator = new DummyServiceGenerator();
                var dummyManager = new DummyServiceDatabaseManager(settings, dummyServiceGenerator);
                var timer = new GeneratorTimer(1000);
                liveGenerator = new LiveGenerator(generator, timer);
                var contextProvider = new ContextProvider();
                var importer = new ServiceDefinitionImporter(manager, ExportableServiceDefinition.CsvRecordPattern);
                var exporter = new ServiceDefinitionExporter();
                var dispatcher = new CommandLineDispatcher(generator, liveGenerator, manager, dummyManager, contextProvider, importer, exporter);
                CommandLineRegistration.Register(contextProvider, dispatcher, parser);

                // Parse the command line
                parser.Parse(args);

                // If help's been requested, show help and exit. Otherwise, dispatch the command
                if (parser.IsPresent(CommandLineOptionType.Help))
                {
                    parser.Help();
                }
                else
                {
                    Console.WriteLine();
                    dispatcher.Dispatch(parser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().Name} : {ex.Message}");
            }
            finally
            {
                liveGenerator?.Dispose();
            }
        }
    }
}
