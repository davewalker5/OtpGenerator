namespace OtpGenerator.Logic.Interfaces
{
    public interface IGeneratorTimer
    {
        event EventHandler<EventArgs> Tick;
        bool Enabled { get; }
        void Start();
        void Stop();
    }
}