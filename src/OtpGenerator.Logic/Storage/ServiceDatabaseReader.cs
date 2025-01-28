using System.Text;
using System.Text.Json;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Cryptography;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Storage
{
    public class ServiceDatabaseReader : IServiceDatabaseReader
    {
        private readonly IDataDecryptor _decryptor = null;

        public ServiceDatabaseReader(IDataDecryptor decryptor)
        {
            _decryptor = decryptor;
        }

        /// <summary>
        /// Read a collection of services from a JSON formatted file 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userSecret"></param>
        public IList<ServiceDefinition> Read(string path, byte[] userSecret)
        {
            // Read the content of the data file
            var content = File.ReadAllBytes(path);

            // Separate the salt and encrypted service definitions
            var salt = content[.. EncryptionKeyGenerator.SaltLength];
            var encrypted = content[EncryptionKeyGenerator.SaltLength ..];

            // Decrypt the result and get a JSON string from the decrypted byte array
            var decrypted = _decryptor.Decrypt(encrypted, userSecret, salt);
            var json = Encoding.UTF8.GetString(decrypted);

            // Deserialise the JSON to a collection of service definitions
            return [.. JsonSerializer.Deserialize<IEnumerable<ServiceDefinition>>(json).OrderBy(x => x.Name)];
        }
    }
}