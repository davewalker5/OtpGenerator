using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Terminal.ResultHandlers
{
    public class ServiceDefinitionTabulator : TabulatorBase, IOperationResultHandler
    {
        private readonly string _maskedSecret = new('*', 10);
        private readonly IContextProvider _contextProvider;

        public ServiceDefinitionTabulator(IContextProvider contextProvider)
            => _contextProvider = contextProvider;

        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="results"></param>
        public void HandleResults(object results)
            => Tabulate(results as IList<ServiceDefinition>);

        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="message"></param>
        public void HandleMessage(string message)
            => Console.WriteLine(message);

        /// <summary>
        /// Generate a table of service definitions
        /// </summary>
        /// <param name="definitions"></param>
        public void Tabulate(IEnumerable<ServiceDefinition> definitions)
        {
            if (definitions?.Count() > 0)
            {
                Initialise();
                SetColumnTitles(["Id", "Service", "Account", "Secret"]);

                foreach (var definition in definitions)
                {
                    var secret = _contextProvider.ShowSecrets ? definition.Secret : _maskedSecret;

                    AddRow([
                        definition.Id.ToString(),
                        definition.Name,
                        definition.Account,
                        secret
                    ], DefaultColour);
                }

                Show();
            }
            else
            {
                Console.WriteLine("No service definitions found");
            }
        }
    }
}
