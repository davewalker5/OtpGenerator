using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Terminal.ResultHandlers
{
    public class DataExchangeResultHandler : IOperationResultHandler
    {
        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="results"></param>
        public void HandleResults(object results)
            => throw new NotImplementedException();

        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="message"></param>
        public void HandleMessage(string message)
            => Console.WriteLine(message);
    }
}