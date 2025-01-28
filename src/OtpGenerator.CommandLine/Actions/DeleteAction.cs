using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public class DeleteAction : ActionBase
    {
        public DeleteAction(
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

            // The service is either specified by name and account or by its Id
            if (values.Count == 1)
            {
                var serviceId = int.Parse(values[0]);
                Manager.DeleteService(serviceId);
            }
            else
            {
                Manager.DeleteService(values[0], values[1]);
            }

            Manager.Save();
            Handler.HandleMessage($"Service '{values[0]}' has been deleted");
        }
    }
}
