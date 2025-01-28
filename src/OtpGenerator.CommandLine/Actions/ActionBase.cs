using OtpGenerator.CommandLine.Interfaces;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public abstract class ActionBase : IAction
    {
        private readonly IServiceDatabaseManager _manager;
        private readonly IServiceDatabaseManager _dummyManager;

        protected IContextProvider ContextProvider { get; private set; }
        protected IServiceDatabaseManager Manager { get { return ContextProvider.Dummy ? _dummyManager : _manager; } }
        protected IOperationResultHandler Handler { get; private set; }

        public ActionBase(
            IContextProvider contextProvider,
            IServiceDatabaseManager manager,
            IServiceDatabaseManager dummyManager,
            IOperationResultHandler handler)
        {
            ContextProvider = contextProvider;
            _manager = manager;
            _dummyManager = dummyManager;
            Handler = handler;
        }

        /// <summary>
        /// Execute the action
        /// </summary>
        /// <param name="contextProvider"></param>
        /// <param name="client"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public abstract void Execute(IList<string> values);

        /// <summary>
        /// Get a list of matching service definitions based on the criteria given in the values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected IList<ServiceDefinition> GetServiceDefinitions(IList<string> values)
        {
            IList<ServiceDefinition> definitions = null;

            // The values may be:
            //
            // []              : Match all services
            // [name]          : Match all services with the specified service name
            // [id]            : Match all services with the specified service Id
            // [name, account] : Match all services with the specified service and account names
            switch (values.Count)
            {
                case 0:
                    definitions = Manager.Services;
                    break;
                case 1:
                    if (int.TryParse(values[0], out int id))
                    {
                        definitions = [Manager.GetService(id)];
                    }
                    else
                    {
                        definitions = Manager.GetServices(values[0]);
                    }
                    break;
                case 2:
                    definitions = [Manager.GetService(values[0], values[1])];
                    break;
            }

            return definitions;
        }
    }
}
