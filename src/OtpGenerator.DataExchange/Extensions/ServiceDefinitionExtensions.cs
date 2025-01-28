using OtpGenerator.DataExchange.Entities;
using OtpGenerator.Entities.Generator;

namespace OtpGenerator.DataExchange.Extensions
{
    public static class ServiceDefinitionExtensions
    {
        /// <summary>
        /// Return an exportable person from a person
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public static ExportableServiceDefinition ToExportable(this ServiceDefinition definition)
            =>  new ExportableServiceDefinition
                {
                    Name = definition.Name,
                    Account = definition.Account,
                    Secret = definition.Secret
                };

        /// <summary>
        /// Return a collection of exportable people from a collection of people
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableServiceDefinition> ToExportable(this IEnumerable<ServiceDefinition> people)
        {
            var exportable = new List<ExportableServiceDefinition>();

            foreach (var person in people)
            {
                exportable.Add(person.ToExportable());
            }

            return exportable;
        }
    }
}
