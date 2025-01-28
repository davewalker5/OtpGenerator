using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.Entities.Generator
{
    [ExcludeFromCodeCoverage]
    public class TotpUpdatedEventArgs : EventArgs
    {
        public TimedOneTimePassword Password { get; set; }
    }
}
