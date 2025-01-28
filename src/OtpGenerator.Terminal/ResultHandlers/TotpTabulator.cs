using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Terminal.ResultHandlers
{
    public class TotpTabulator : TabulatorBase, IOperationResultHandler
    {
        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="results"></param>
        public void HandleResults(object results)
            => Tabulate(results as IList<TimedOneTimePassword>);

        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="message"></param>
        public void HandleMessage(string message)
            => Console.WriteLine(message);

        /// <summary>
        /// Generate a table of TOTPs
        /// </summary>
        /// <param name="totps"></param>
        public void Tabulate(IList<TimedOneTimePassword> totps)
        {
            if (totps?.Count > 0)
            {
                Initialise();
                SetColumnTitles(["Id", "Service", "Account", "Code", "Remaining Seconds"]);

                foreach (var totp in totps)
                {
                    AddRow([
                        totp.ServiceId.ToString(),
                        totp.ServiceName,
                        totp.Account,
                        $"{totp.Code[..3]} {totp.Code[3..]}",
                        totp.RemainingSeconds.ToString()
                    ], DefaultColour);
                }

                Show();
            }
            else
            {
                Console.WriteLine("No TOTPs found");
            }
        }
    }
}
