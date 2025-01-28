using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public class MockAction : ActionBase
    {
        public Type ManagerType { get { return Manager.GetType(); } }

        public MockAction(
            IContextProvider contextProvider,
            IServiceDatabaseManager manager,
            IServiceDatabaseManager dummyManager) : base(contextProvider, manager, dummyManager, null)
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
        }
    }
}
