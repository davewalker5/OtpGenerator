using System.Collections.ObjectModel;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Generator
{
    public class LiveGenerator : ILiveGenerator, IDisposable
    {
        public event EventHandler<TotpUpdatedEventArgs> TotpUpdated;
        private readonly ICodeGenerator _generator;
        private readonly List<ServiceDefinition> _definitions = [];
        private readonly IGeneratorTimer _timer;

        public ReadOnlyCollection<ServiceDefinition> Services { get { return _definitions.AsReadOnly(); } }

        public LiveGenerator(ICodeGenerator generator, IGeneratorTimer timer)
        {
            _generator = generator;
            _timer = timer;
        }

        /// <summary>
        /// Add a service to the list being monitored
        /// </summary>
        /// <param name="definition"></param>
        public void AddService(ServiceDefinition definition)
        {
            Stop();
            _definitions.Add(definition);
        }

        /// <summary>
        /// Add a collection of services to the list being monitored
        /// </summary>
        /// <param name="definitions"></param>
        public void AddServices(IEnumerable<ServiceDefinition> definitions)
        {
            Stop();
            _definitions.AddRange(definitions);
        }

        /// <summary>
        /// Clear the current list of definitions
        /// </summary>
        public void ClearServices()
        {
            Stop();
            _definitions.Clear();
        }

        /// <summary>
        /// Register the tick handler and start the timer
        /// </summary>
        public void Start()
        {
            if (!_timer.Enabled)
            {
                _timer.Tick += OnTimerTick;
                _timer.Start();
            }
        }

        /// <summary>
        /// Stop the timer and un-register the tick handler
        /// </summary>
        public void Stop()
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
                _timer.Tick -= OnTimerTick;
            }
        }

        /// <summary>
        /// Timer tick callback handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            foreach (var definition in _definitions)
            {
                var totp = _generator.Generate(definition);
                TotpUpdated?.Invoke(this, new TotpUpdatedEventArgs() { Password = totp });
            }
        }

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }
    }
}