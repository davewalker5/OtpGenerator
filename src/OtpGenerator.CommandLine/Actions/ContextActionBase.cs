using OtpGenerator.CommandLine.Interfaces;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Actions
{
    public abstract class ContextActionBase : IAction
    {
        protected readonly IContextProvider _provider;
        protected readonly IOperationResultHandler _handler;

        public ContextActionBase(IContextProvider provider, IOperationResultHandler handler)
        {
            _provider = provider;
            _handler = handler;
        }

        /// <summary>
        /// Execute the action
        /// </summary>
        /// <param name="contextProvider"></param>
        /// <param name="client"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        
        public abstract void Execute(IList<string> values);
    }
}
