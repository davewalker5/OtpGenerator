using OtpGenerator.Logic.Generator;
using OtpGenerator.Tests.Mocks;

namespace OtpGenerator.Tests.TotpTests
{
    [TestClass]
    public class DummyServiceGeneratorTest
    {
        [TestMethod]
        public void GenerateServiceDefinitionsTest()
        {
            var name = DataGenerator.RandomServiceName();
            var account = DataGenerator.RandomAccountName();
            var numberOfServices = DataGenerator.RandomInt(5, 20);

            var definitions = new DummyServiceGenerator().GenerateDefinitions(name, account, numberOfServices);

            foreach (var definition in definitions)
            {
                Assert.IsTrue(definition.Id > 0);
                Assert.IsTrue(definition.Name.Contains(name));
                Assert.IsTrue(definition.Account.Contains(account));
                Assert.IsFalse(string.IsNullOrEmpty(definition.Secret));
            }
        }
    }
}