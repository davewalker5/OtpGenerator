using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;
using System.Text;
using OtpGenerator.Entities.Exceptions;
using OtpGenerator.Tests.Mocks;

namespace OtpGenerator.Tests.CryptographyTests
{
    [TestClass]
    public class EncryptorDecryptorTest
    {
        private byte[] _userSecret;
        private byte[] _salt;
        private string _data;
        private byte[] _dataBytes;
        private IDataEncryptor _encryptor;
        private IDataDecryptor _decryptor;

        [TestInitialize]
        public void Initialise()
        {
            var settings = DataGenerator.MockApplicationSettings();
            var keyGenerator = new EncryptionKeyGenerator(settings);

            _decryptor = new DataDecryptor(keyGenerator);
            _encryptor = new DataEncryptor(keyGenerator);

            _userSecret = DataGenerator.RandomUserSecret();
            _salt = keyGenerator.GenerateSalt();
            _data = DataGenerator.RandomWord(10, 100);
            _dataBytes = Encoding.UTF8.GetBytes(_data);
        }

        [TestMethod]
        public void EncryptDecryptTest()
        {
            (var encrypted, var salt) = _encryptor.Encrypt(_dataBytes, _userSecret);
            var decrypted = Encoding.UTF8.GetString(_decryptor.Decrypt(encrypted, _userSecret, salt));

            Assert.AreEqual(_data, decrypted);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataForEncryptionDecryptionException))]
        public void CannotEncryptNullDataTest()
            => _encryptor.Encrypt(null, _userSecret);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataForEncryptionDecryptionException))]
        public void CannotEncryptEmptyDataTest()
            => _encryptor.Encrypt([], _userSecret);

        [TestMethod]
        [ExpectedException(typeof(InvalidUserSecretException))]
        public void CannotEncryptWithNullUserSecretTest()
            => _encryptor.Encrypt(_dataBytes, null);

        [TestMethod]
        [ExpectedException(typeof(InvalidUserSecretException))]
        public void CannotEncryptWithEmptyUserSecretTest()
            => _encryptor.Encrypt(_dataBytes, []);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataForEncryptionDecryptionException))]
        public void CannotDecryptNullDataTest()
            => _decryptor.Decrypt(null, _userSecret, _salt);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataForEncryptionDecryptionException))]
        public void CannotDecryptEmptyDataTest()
            => _decryptor.Decrypt([], _userSecret, _salt);

        [TestMethod]
        [ExpectedException(typeof(InvalidUserSecretException))]
        public void CannotDecryptWithNullUserSecretTest()
            => _decryptor.Decrypt(_dataBytes, null, _salt);

        [TestMethod]
        [ExpectedException(typeof(InvalidUserSecretException))]
        public void CannotDecryptWithEmptyUserSecretTest()
            => _decryptor.Decrypt(_dataBytes, [], _salt);
    }
}