using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.Entities.Generator
{
    [ExcludeFromCodeCoverage]
    public class ServiceDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Account { get; set;}
        public string Secret { get; set; }
    }
}