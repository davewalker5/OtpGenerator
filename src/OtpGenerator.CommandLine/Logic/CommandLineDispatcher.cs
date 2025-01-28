using OtpGenerator.Logic.Interfaces;
using OtpGenerator.CommandLine.Actions;
using OtpGenerator.CommandLine.Interfaces;
using OtpGenerator.CommandLine.Entities;
using OtpGenerator.DataExchange.Interfaces;

namespace OtpGenerator.CommandLine.Logic
{
    public class CommandLineDispatcher : ICommandLineDispatcher
    {
        private readonly Dictionary<CommandLineOptionType, IAction> _handlers = [];
        private readonly ICodeGenerator _generator;
        private readonly ILiveGenerator _liveGenerator;
        private readonly IServiceDatabaseManager _manager;
        private readonly IServiceDatabaseManager _dummyManager;
        private readonly IContextProvider _contextProvider;
        private readonly IServiceDefinitionImporter _importer;
        private readonly IServiceDefinitionExporter _exporter;

        public CommandLineDispatcher(
            ICodeGenerator generator,
            ILiveGenerator liveGenerator,
            IServiceDatabaseManager manager,
            IServiceDatabaseManager dummyManager,
            IContextProvider contextProvider,
            IServiceDefinitionImporter importer,
            IServiceDefinitionExporter exporter)
        {
            _generator = generator;
            _liveGenerator = liveGenerator;
            _manager = manager;
            _dummyManager = dummyManager;
            _contextProvider = contextProvider;
            _importer = importer; 
            _exporter = exporter;
        }

        /// <summary>
        /// Register the handler for the "add" operation
        /// </summary>
        /// <param name="resultHandler"></param>
        public void RegisterAddCommandHandler(IOperationResultHandler resultHandler)
            => _handlers.Add(CommandLineOptionType.Add, new AddAction(_contextProvider, _generator, _manager, _dummyManager, resultHandler));

        /// <summary>
        /// Register the handler for the "delete" operation
        /// </summary>
        /// <param name="resultHandler"></param>
        public void RegisterDeleteCommandHandler(IOperationResultHandler resultHandler)
            => _handlers.Add(CommandLineOptionType.Delete, new DeleteAction(_contextProvider, _manager, _dummyManager, resultHandler));

        /// <summary>
        /// Register the handler for the "list" operation
        /// </summary>
        /// <param name="resultHandler"></param>
        public void RegisterListCommandHandler(IOperationResultHandler resultHandler)
            => _handlers.Add(CommandLineOptionType.List, new ListAction(_contextProvider, _manager, _dummyManager, resultHandler));

        /// <summary>
        /// Register the handler for the "generate" operation
        /// </summary>
        /// <param name="resultHandler"></param>
        public void RegisterGenerateCommandHandler(IOperationResultHandler resultHandler)
            => _handlers.Add(CommandLineOptionType.Generate, new GenerateAction(_contextProvider, _generator, _manager, _dummyManager, resultHandler));

        /// <summary>
        /// Register the handler for the "generate all" operation
        /// </summary>
        /// <param name="resultHandler"></param>
        public void RegisterImportCommandHandler(IOperationResultHandler resultHandler)
            => _handlers.Add(CommandLineOptionType.Import, new ImportAction(_contextProvider, _manager, _dummyManager, _importer, resultHandler));

        /// <summary>
        /// Register the handler for the "generate all" operation
        /// </summary>
        /// <param name="resultHandler"></param>
        public void RegisterExportCommandHandler(IOperationResultHandler resultHandler)
            => _handlers.Add(CommandLineOptionType.Export, new ExportAction(_contextProvider, _manager, _dummyManager, _exporter, resultHandler));

        /// <summary>
        /// Register the handler for the "live view" operation
        /// </summary>
        /// <param name="resultHandler"></param>
        public void RegisterLiveViewCommandHandler(IOperationResultHandler resultHandler)
            => _handlers.Add(CommandLineOptionType.LiveView, new LiveViewAction(_contextProvider, _liveGenerator, _manager, _dummyManager, resultHandler));

        /// <summary>
        /// Register the handler for the "show secrets" context option
        /// </summary>
        /// <param name="resultHandler"></param>
        public void RegisterVerboseContextOptionHandler(IOperationResultHandler resultHandler)
            => _handlers.Add(CommandLineOptionType.Verbose, new VerboseAction(_contextProvider, resultHandler));

        /// <summary>
        /// Register the handler for the "show secrets" context option
        /// </summary>
        /// <param name="resultHandler"></param>
        public void RegisterShowSecretsContextOptionHandler(IOperationResultHandler resultHandler)
            => _handlers.Add(CommandLineOptionType.ShowSecrets, new ShowSecretsAction(_contextProvider, resultHandler));

        /// <summary>
        /// Register the handler for the "dummy" context option
        /// </summary>
        /// <param name="resultHandler"></param>
        public void RegisterDummyContextOptionHandler(IOperationResultHandler resultHandler)
            => _handlers.Add(CommandLineOptionType.Dummy, new DummyAction(_contextProvider, resultHandler));

        /// <summary>
        /// Dispatch the commands given on a command line that has already been parsed
        /// </summary>
        /// <param name="parser"></param>
        public void Dispatch(ICommandLineParser parser)
        {
            // Dispatch is in two phases. Options for manipulating the context are dispatched first, as
            // they set up the context for the operations that are dispatched second
            Dispatch(parser, parser.GetSpecifiedOptions(false, true));
            Dispatch(parser, parser.GetSpecifiedOptions(true, false));
        }

        /// <summary>
        /// Dispatch a set of operations/context options
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="options"></param>
        private void Dispatch(ICommandLineParser parser, IList<CommandLineOptionType> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                // Find the handler for this option
                var handler = _handlers[options[i]];

                // Extract the values and handle the command
                var values = parser.GetValues(options[i]);
                handler.Execute(values);
            }
        }
    }
}
