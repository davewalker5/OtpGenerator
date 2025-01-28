namespace OtpGenerator.Configuration.Interfaces
{
    public interface IOtpGeneratorApplicationSettings
    {
        string DatabasePath { get; set; }
        int KeyGenerationParallelism { get; set; }
        int KeyGenerationMemory { get; set; }
        int KeyGenerationIterations { get; set; }
        string DummyServiceName { get; set; }
        string DummyServiceAccount { get; set; }
        int NumberOfDummyServices { get; set; }
    }
}
