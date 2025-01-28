using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public class ListAction : ActionBase
    {
        public ListAction(
            IContextProvider contextProvider,
            IServiceDatabaseManager manager,
            IServiceDatabaseManager dummyManager,
            IOperationResultHandler handler) : base(contextProvider, manager, dummyManager, handler)
        {
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
            Manager.Load();

            // Determine which services match the criteria given in the values
            var definitions = GetServiceDefinitions(values);

            // Pass the results to the result handler
            Handler.HandleResults(definitions);
        }
    }
}
