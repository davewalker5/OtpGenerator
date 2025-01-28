using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public class VerboseAction : ContextActionBase
    {
        public VerboseAction(IContextProvider provider, IOperationResultHandler handler) : base(provider, handler)
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
            _provider.Verbose = true;
            _handler.HandleResults(_provider);
        }
    }
}
