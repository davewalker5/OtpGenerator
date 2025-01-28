using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Terminal.ResultHandlers
{
    public class ServiceDeletionHandler : TabulatorBase, IOperationResultHandler
    {
        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="results"></param>
        public void HandleResults(object results)
        {
            var message = $"{this.GetType().Name} cannot handle an object result";
            throw new NotImplementedException(message);
        }

        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="message"></param>
        public void HandleMessage(string message)
            => Console.WriteLine(message);
    }
}
