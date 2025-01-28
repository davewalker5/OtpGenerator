using OtpGenerator.DataExchange.Entities;
using OtpGenerator.DataExchange.Interfaces;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.DataExchange.Import
{
    public sealed class ServiceDefinitionImporter : CsvImporter<ExportableServiceDefinition>, IServiceDefinitionImporter
    {
        private readonly IServiceDatabaseManager _manager;

        public ServiceDefinitionImporter(IServiceDatabaseManager manager, string format) : base (format)
            => _manager = manager;

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableServiceDefinition Inflate(string record)
            => ExportableServiceDefinition.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected override void Validate(ExportableServiceDefinition definition, int recordCount)
        {
            ValidateField<string>(x => !string.IsNullOrEmpty(x), definition.Name,  "Name", recordCount);
            ValidateField<string>(x => !string.IsNullOrEmpty(x), definition.Account,  "Account", recordCount);
            ValidateField<string>(x => !string.IsNullOrEmpty(x), definition.Secret,  "Secret", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        protected override void Add(ExportableServiceDefinition definition)
            => _manager.AddService(definition.Name, definition.Account, definition.Secret);
    }
}