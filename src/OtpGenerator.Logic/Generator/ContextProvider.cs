using System.Diagnostics.CodeAnalysis;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Generator
{
    [ExcludeFromCodeCoverage]
    public class ContextProvider : IContextProvider
    {
        public bool Verbose { get; set; } = false;
        public bool ShowSecrets { get; set; } = false;
        public bool Dummy { get; set; } = false;
    }
}