using OtpGenerator.Entities.Exceptions;
using OtpGenerator.Logic.Generator;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Tests.Mocks;

namespace OtpGenerator.Tests.TotpTests
{
    [TestClass]
    public class CodeGeneratorTest
    {
        private ICodeGenerator _generator = new CodeGenerator();

        [TestMethod]
        public void GenerateCodeTest()
        {
            var secret = DataGenerator.RandomServiceSecret();
            var totp = _generator.Generate(new() { Secret = secret });
       
            Assert.IsNotNull(totp);
            Assert.IsFalse(string.IsNullOrEmpty(totp.Code));
            Assert.AreEqual(6, totp.Code.Length);
            Assert.IsTrue(int.TryParse(totp.Code, out int intCode));
            Assert.IsTrue(totp.RemainingSeconds >= 0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidServiceSecretException))]
        public void CannotGenerateCodeWithNullSecret()
            => _generator.Generate(new() { Secret = null });

        [TestMethod]
        [ExpectedException(typeof(InvalidServiceSecretException))]
        public void CannotGenerateCodeWithEmptySecret()
            => _generator.Generate(new() { Secret = "" });
    }
}