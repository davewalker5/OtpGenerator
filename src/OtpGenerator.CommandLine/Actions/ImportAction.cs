using OtpGenerator.DataExchange.Entities;
using OtpGenerator.DataExchange.Interfaces;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public class ImportAction : ActionBase
    {
        private readonly IContextProvider _contextProvider;
        private readonly IServiceDefinitionImporter _importer;

        public ImportAction(
            IContextProvider contextProvider,
            IServiceDatabaseManager manager,
            IServiceDatabaseManager dummyManager,
            IServiceDefinitionImporter importer,
            IOperationResultHandler handler) : base(contextProvider, manager, dummyManager, handler)
        {
            _contextProvider = contextProvider;
            _importer = importer;
        }


        /// <summary>
        /// Execute the action
        /// </summary>
        /// <param name="contextProvider"></param>
        /// <param name="client"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public override void Execute(IList<string> values)
        {
            // If verbose output is enabled, register the "record imported" callback
            if (_contextProvider.Verbose)
            {
                _importer.RecordImport += OnRecordImported;
            }

            // Load the current database, import the additional entries and re-save
            Manager.Load();
            _importer.Import(values[0]);
            Manager.Save();

            // If verbose output is enabled, un-register the "record imported" callback
            if (_contextProvider.Verbose)
            {
                _importer.RecordImport -= OnRecordImported;
            }

            // Send a closing message
            Handler.HandleMessage($"Import of service definitions from {values[0]} completed.");
        }

        /// <summary>
        /// Handle a record import notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecordImported(object sender, ImportEventArgs<ExportableServiceDefinition> e)
            => Handler.HandleMessage($"Imported definition for account '{e.Entity.Account}' at service '{e.Entity.Name}'");
    }
}
