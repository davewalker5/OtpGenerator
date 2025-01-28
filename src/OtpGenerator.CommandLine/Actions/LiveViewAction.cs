using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public class LiveViewAction : ActionBase
    {
        private readonly ILiveGenerator _generator;

        public LiveViewAction(
            IContextProvider contextProvider,
            ILiveGenerator generator,
            IServiceDatabaseManager manager,
            IServiceDatabaseManager dummyManager,
            IOperationResultHandler handler) : base(contextProvider, manager, dummyManager, handler)
        {
            _generator = generator;
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
            // Load the database and clear any existing services registered with the generator
            Manager.Load();
            _generator.ClearServices();

            // Determine which services match the criteria given in the values and add them to the generator
            var definitions = GetServiceDefinitions(values);
            _generator.AddServices(definitions);

            // Pass the live generator to the results handler to start ongoing generation
            Handler.HandleResults(_generator);
        }
    }
}
