using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Terminal.ResultHandlers
{
    public class ContextTabulator : TabulatorBase, IOperationResultHandler
    {
        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="results"></param>
        public void HandleResults(object results)
            => Tabulate(results as IContextProvider);

        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="message"></param>
        public void HandleMessage(string message)
            => Console.WriteLine(message);

        /// <summary>
        /// Show the current context
        /// </summary>
        /// <param name="provider"></param>
        public void Tabulate(IContextProvider provider)
        {
            // Only tabulate the context if the verbose option is set
            if (provider.Verbose)
            {
                Initialise();
                SetColumnTitles(["Property", "Value"]);
                AddRow(["Dummy Mode", provider.Dummy.ToString()], DefaultColour);
                AddRow(["Show Secrets", provider.ShowSecrets.ToString()], DefaultColour);
                AddRow(["Verbose", provider.Verbose.ToString()], DefaultColour);
                Show();
            }
        }
    }
}