using OtpGenerator.DataExchange.Entities;
using OtpGenerator.DataExchange.Interfaces;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public class ExportAction : ActionBase
    {        private readonly IServiceDefinitionExporter _exporter;

        public ExportAction(
            IContextProvider contextProvider,
            IServiceDatabaseManager manager,
            IServiceDatabaseManager dummyManager,
            IServiceDefinitionExporter exporter,
            IOperationResultHandler handler) : base(contextProvider, manager, dummyManager, handler)
            => _exporter = exporter;

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
            if (ContextProvider.Verbose)
            {
                _exporter.RecordExport += OnRecordExported;
            }

            // Load and export the service definitions
            Manager.Load();
            _exporter.Export(Manager.Services, values[0]);

            // If verbose output is enabled, un-register the "record imported" callback
            if (ContextProvider.Verbose)
            {
                _exporter.RecordExport -= OnRecordExported;
            }

            // Send a closing message
            Handler.HandleMessage($"Export of service definitions to {values[0]} completed.");
        }

        /// <summary>
        /// Handle a record export notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object sender, ExportEventArgs<ExportableServiceDefinition> e)
            => Handler.HandleMessage($"Exported definition for account '{e.RecordSource.Account}' at service '{e.RecordSource.Name}'");
    }
}
