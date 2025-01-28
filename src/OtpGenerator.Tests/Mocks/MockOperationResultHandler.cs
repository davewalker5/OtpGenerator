using System.Collections.ObjectModel;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Tests.Mocks
{
    public class MockOperationResultHandler : IOperationResultHandler
    {
        private readonly List<object> _objectResults = [];
        private readonly List<string> _messageResults = [];

        public IList<object> ObjectResults { get { return _objectResults; } }
        public IList<string> MessageResults { get { return _messageResults; } }

        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="results"></param>
        public void HandleResults(object results)
        {
            Console.WriteLine($"Handling {results.GetType().Name} result ...");
            _objectResults.Add(results);
        }

        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="message"></param>
        public void HandleMessage(string message)
            => _messageResults.Add(message);
    }
}