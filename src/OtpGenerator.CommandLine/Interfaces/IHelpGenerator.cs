using OtpGenerator.CommandLine.Entities;

namespace OtpGenerator.CommandLine.Interfaces
{
    public interface IHelpGenerator
    {
        void Generate(IEnumerable<CommandLineOption> options);
    }
}
