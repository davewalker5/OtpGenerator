using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Interfaces;
using Spectre.Console;

namespace OtpGenerator.Terminal.ResultHandlers
{
    public class LiveView : TabulatorBase, IOperationResultHandler
    {
        private Dictionary<int, int> _mapping;

        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="results"></param>
        public void HandleResults(object results)
            => StartLiveView(results as ILiveGenerator);

        /// <summary>
        /// Callback method for the dispatcher
        /// </summary>
        /// <param name="message"></param>
        public void HandleMessage(string message)
            => throw new NotImplementedException();

        /// <summary>
        /// Start a live view
        /// </summary>
        /// <param name="generator"></param>
        public void StartLiveView(ILiveGenerator generator)
        {
            // Create a mapping between table row index and service Id
            _mapping = generator.Services
                                .Select((service, index) => new { service.Id, Index = index })
                                .ToDictionary(x => x.Id, x => x.Index);

            // Initialise the table
            InitialiseLiveTable(generator);

            // Subscribe to the "code updated events" and start the generator
            generator.TotpUpdated += OnTotpUpdated;
            generator.Start();

            // Run the live view
            Task.Run(() => 
                AnsiConsole
                    .Live(Table)
                    .AutoClear(true)
                    .Overflow(VerticalOverflow.Ellipsis)
                    .Cropping(VerticalOverflowCropping.Bottom)
                    .StartAsync(async ctx =>
                    {
                        await RunTableRefreshLoop(ctx);
                    })
            ).Wait();

            // Stop the generator and unsubscribe from TOTP updated events
            generator.Stop();
            generator.TotpUpdated -= OnTotpUpdated;
        }

        /// <summary>
        /// Initialise the table
        /// </summary>
        /// <param name="generator"></param>
        private void InitialiseLiveTable(ILiveGenerator generator)
        {
            // Initialise the table
            Initialise();
            SetColumnTitles(["Id", "Service", "Account", "Code", "Remaining Seconds"]);

            // Add the initial service entries to the table
            foreach (var service in generator.Services)
            {
                AddRow([
                    service.Id.ToString(),
                    service.Name,
                    service.Account,
                    "",
                    ""
                ], DefaultColour);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private async Task RunTableRefreshLoop(LiveDisplayContext ctx)
        {
            bool userEscaped = false;

            do
            {
                // Refresh then wait a while to avoid a busy loop
                ctx.Refresh();
                await Task.Delay(100);

                // See if there's a keypress waiting
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(false);
                    userEscaped = key.Key == ConsoleKey.Escape;
                }
            }
            while (!userEscaped);
        }

        /// <summary>
        /// Handler for TOTP updated events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTotpUpdated(object sender, TotpUpdatedEventArgs args)
        {
            // Generate the row data
            List<string> values =
            [
                args.Password.ServiceId.ToString(),
                args.Password.ServiceName,
                args.Password.Account,
                $"{args.Password.Code[..3]} {args.Password.Code[3..]}",
                args.Password.RemainingSeconds.ToString()
            ];

            // Get the index for the row in the table, remove it and reinsert the new one
            var rowNumber = _mapping[args.Password.ServiceId];
            RemoveRow(rowNumber);
            InsertRow(rowNumber, values, DefaultColour);
        }
    }
}