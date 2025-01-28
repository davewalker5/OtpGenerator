using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;
using OtpGenerator.Tests.Mocks;

namespace OtpGenerator.Tests.CryptographyTests
{
    [TestClass]
    public class EncryptionKeyGeneratorTest
    {
        private IEncryptionKeyGenerator _generator;

        [TestInitialize]
        public void Initialise()
        {
            var settings = DataGenerator.MockApplicationSettings();
            _generator = new EncryptionKeyGenerator(settings);
        }

        [TestMethod]
        public void GeneratedSaltIsUniqueTest()
        {
            List<byte[]> salts = [];

            for (int i = 0; i < 100; i++)
            {
                var salt = _generator.GenerateSalt();
                Assert.AreEqual(EncryptionKeyGenerator.SaltLength, salt.Length);

                bool existing = salts.Any(x => x.SequenceEqual(salt));
                Assert.IsFalse(existing);
            }
        }

        [TestMethod]
        public void SameSecretAndSameSaltYieldSameKeyTest()
        {
            var userSecret = DataGenerator.RandomUserSecret();
            var salt = _generator.GenerateSalt();
            var firstKey = Convert.ToHexString(_generator.GenerateKey(userSecret, salt));
            var secondKey = Convert.ToHexString(_generator.GenerateKey(userSecret, salt));
            Assert.AreEqual(firstKey, secondKey);
        }

        [TestMethod]
        public void SameSecretAndDifferentSaltYieldDifferentKeyTest()
        {
            var userSecret = DataGenerator.RandomUserSecret();
            var firstSalt = _generator.GenerateSalt();
            var firstKey = Convert.ToHexString(_generator.GenerateKey(userSecret, firstSalt));
            var secondSalt = _generator.GenerateSalt();
            var secondKey = Convert.ToHexString(_generator.GenerateKey(userSecret, secondSalt));
            Assert.AreNotEqual(firstSalt, secondSalt);
            Assert.AreNotEqual(firstKey, secondKey);
        }

        [TestMethod]
        public void DifferentSecretAndSameSaltYieldDifferentKeyTest()
        {
            var salt = _generator.GenerateSalt();
            var firstSecret = DataGenerator.RandomUserSecret();
            var firstKey = Convert.ToHexString(_generator.GenerateKey(firstSecret, salt));
            var secondSecret = DataGenerator.RandomUserSecret();
            var secondKey = Convert.ToHexString(_generator.GenerateKey(secondSecret, salt));
            Assert.AreNotEqual(firstSecret, secondSecret);
            Assert.AreNotEqual(firstKey, secondKey);
        }
    }
}