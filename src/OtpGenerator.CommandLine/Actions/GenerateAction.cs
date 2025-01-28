using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public class GenerateAction : ActionBase
    {
        private readonly ICodeGenerator _generator;

        public GenerateAction(
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
            Manager.Load();

            // Determine which services match the criteria given in the values
            var definitions = GetServiceDefinitions(values);

            // Generate the TOTPs
            List<TimedOneTimePassword> results = [];
            foreach (var definition in definitions)
            {
                var totp = _generator.Generate(definition);
                results.Add(totp);
            }

            Handler.HandleResults(results);
        }
    }
}
