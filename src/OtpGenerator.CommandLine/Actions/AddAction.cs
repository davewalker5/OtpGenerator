using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public class AddAction : ActionBase
    {
        private readonly ICodeGenerator _generator;

        public AddAction(
            IContextProvider contextProvider,
            ICodeGenerator generator,
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
            // When adding a new service, it's added to the database first then a new TOTP is generated
            // and displayed
            Manager.Load();
            var service = Manager.AddService(values[0], values[1], values[2].Replace(" ", ""));
            var totp = _generator.Generate(service);
            Manager.Save();

            List<TimedOneTimePassword> results = [totp];
            Handler.HandleResults(results);
        }
    }
}
