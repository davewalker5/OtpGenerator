using OtpGenerator.Configuration.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.Configuration.Entities
{
    [ExcludeFromCodeCoverage]
    public class OtpGeneratorApplicationSettings : IOtpGeneratorApplicationSettings
    {
        public string DatabasePath { get; set; }
        public int KeyGenerationParallelism { get; set; }
        public int KeyGenerationMemory { get; set; }
        public int KeyGenerationIterations { get; set; }
        public string DummyServiceName { get; set; }
        public string DummyServiceAccount { get; set; }
        public int NumberOfDummyServices { get; set; }
    }
}
