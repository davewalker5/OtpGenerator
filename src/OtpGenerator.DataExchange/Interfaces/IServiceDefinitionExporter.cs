using OtpGenerator.DataExchange.Entities;
using OtpGenerator.Entities.Generator;

namespace OtpGenerator.DataExchange.Interfaces
{
    public interface IServiceDefinitionExporter
    {
        event EventHandler<ExportEventArgs<ExportableServiceDefinition>> RecordExport;
        void Export(IEnumerable<ServiceDefinition> definitions, string file);
    }
}
