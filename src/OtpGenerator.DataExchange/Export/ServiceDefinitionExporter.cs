using OtpGenerator.DataExchange.Interfaces;
using OtpGenerator.DataExchange.Extensions;
using System.Diagnostics.CodeAnalysis;
using OtpGenerator.DataExchange.Entities;
using OtpGenerator.Entities.Generator;

namespace OtpGenerator.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class ServiceDefinitionExporter : IServiceDefinitionExporter
    {
        public event EventHandler<ExportEventArgs<ExportableServiceDefinition>> RecordExport;

        /// <summary>
        /// Export a collection of service definitions to a CSV file
        /// </summary>
        /// <param name="definitions"></param>
        /// <param name="file"></param>
        public void Export(IEnumerable<ServiceDefinition> definitions, string file)
        {
            // Convert the collection to "exportable" equivalents with all properties at the same level
            IEnumerable<ExportableServiceDefinition> exportable = definitions.ToExportable();

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableServiceDefinition>();
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }

        /// <summary>
        /// Handler for person export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableServiceDefinition> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}
