using System.Diagnostics.CodeAnalysis;
using System.Timers;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Generator
{
    [ExcludeFromCodeCoverage]
    public class GeneratorTimer : IGeneratorTimer
    {
        private System.Timers.Timer _timer = null;
        private readonly double _interval;

        public event EventHandler<EventArgs> Tick = null;

        public bool Enabled { get { return _timer != null;}}

        public GeneratorTimer(double interval)
        {
            _interval = interval;
        }

        public void Start()
        {
            _timer = new System.Timers.Timer(interval: _interval);
            _timer.Elapsed += OnElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
        }

        public void Stop()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            Tick?.Invoke(this, e);
        }
    }
}
