using OtpGenerator.CommandLine.Entities;
using OtpGenerator.CommandLine.Interfaces;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Terminal.ResultHandlers;

namespace OtpGenerator.Terminal
{
    public static class CommandLineRegistration
    {
        /// <summary>
        /// Register command line options and dispatcher
        /// </summary>
        /// <param name="contextProvider"></param>
        /// <param name="clientProvider"></param>
        /// <param name="parser"></param>
        /// <param name="dispatcher"></param>
        public static void Register(
            IContextProvider contextProvider,
            ICommandLineDispatcher dispatcher,
            ICommandLineParser parser)
        {
            // Register the commands
            parser.Add(CommandLineOptionType.Help, true, "--help", "-h", "Show command line help", "", 0, 0);
            parser.Add(CommandLineOptionType.Add, true, "--add", "-a", "Add a new service definition", "name account secret", 3, 3);
            parser.Add(CommandLineOptionType.Delete, true, "--delete", "-d", "Delete an existing service definition", "id | name account", 1, 2);
            parser.Add(CommandLineOptionType.List, true, "--list", "-l", "List the current service definitions", "[[id | name | name account]]", 0, 2);
            parser.Add(CommandLineOptionType.Generate, true, "--generate", "-g", "Generate a TOTP for an account", "[[id | name | name account]]", 0, 2);
            parser.Add(CommandLineOptionType.LiveView, true, "--live", "-li", "Show a live view for a service", "[[id | name | name account]]", 0, 2);
            parser.Add(CommandLineOptionType.Import, true, "--import", "-i", "Import service definitions from a CSV file", "filepath", 1, 1);
            parser.Add(CommandLineOptionType.Export, true, "--export", "-e", "Export service definitions to a CSV file", "filepath", 1, 1);

            // Register context manipulation options
            parser.Add(CommandLineOptionType.Verbose, false, "--verbose", "-v", "Enable verbose output", "", 0, 0);
            parser.Add(CommandLineOptionType.ShowSecrets, false, "--show-secrets", "-sse", "Enable display of secrets", "", 0, 0);
            parser.Add(CommandLineOptionType.Dummy, false, "--dummy", "-du", "Run in dummy mode, with sumulated service data", "", 0, 0);

            // Configure the dispatcher
            var totpTabulator = new TotpTabulator();
            dispatcher.RegisterAddCommandHandler(totpTabulator);
            dispatcher.RegisterListCommandHandler(new ServiceDefinitionTabulator(contextProvider));
            dispatcher.RegisterGenerateCommandHandler(totpTabulator);
            dispatcher.RegisterDeleteCommandHandler(new ServiceDeletionHandler());

            var dataExchangeResultsHandler = new DataExchangeResultHandler();
            dispatcher.RegisterImportCommandHandler(dataExchangeResultsHandler);
            dispatcher.RegisterExportCommandHandler(dataExchangeResultsHandler);

            var liveViewHandler = new LiveView();
            dispatcher.RegisterLiveViewCommandHandler(liveViewHandler);

            var contextTabulator = new ContextTabulator();
            dispatcher.RegisterVerboseContextOptionHandler(contextTabulator);
            dispatcher.RegisterShowSecretsContextOptionHandler(contextTabulator);
            dispatcher.RegisterDummyContextOptionHandler(contextTabulator);
        }
    }
}
