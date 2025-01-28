using OtpGenerator.Configuration.Logic;

namespace OtpGenerator.Tests.ConfigTests
{
    [TestClass]
    public class OtpGeneratorConfigReaderTest
    {
        [TestMethod]
        public void ReadAppSettingsTest()
        {
            var settings = new OtpGeneratorConfigReader().Read("appsettings.json");

            Assert.AreEqual("/path/to/otp/database", settings.DatabasePath);
            Assert.AreEqual(4, settings.KeyGenerationParallelism);
            Assert.AreEqual(262144, settings.KeyGenerationMemory);
            Assert.AreEqual(10, settings.KeyGenerationIterations);
            Assert.AreEqual("Service", settings.DummyServiceName);
            Assert.AreEqual("someone@some.domain.com", settings.DummyServiceAccount);
            Assert.AreEqual(5, settings.NumberOfDummyServices);
        }
    }
}
