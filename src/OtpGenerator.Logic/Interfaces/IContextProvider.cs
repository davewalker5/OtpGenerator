namespace OtpGenerator.Logic.Interfaces
{
    public interface IContextProvider
    {
        bool Verbose { get; set; }
        bool ShowSecrets { get; set; }
        bool Dummy { get; set; }
    }
}
