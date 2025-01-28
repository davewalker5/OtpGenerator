using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.CommandLine.Entities
{
    [ExcludeFromCodeCoverage]
    public class CommandLineOptionValue
    {
        public CommandLineOption Option { get; set; }
        public List<string> Values { get; private set; } = [];
    }
}
