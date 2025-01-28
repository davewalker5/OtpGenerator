using System.Text;
using System.Text.Json;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Storage
{
    public class ServiceDatabaseWriter : IServiceDatabaseWriter
    {
        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
        private readonly IDataEncryptor _encryptor = null;

        public ServiceDatabaseWriter(IDataEncryptor encryptor)
            => _encryptor = encryptor;

        /// <summary>
        /// Write a set of service definitions to a file in JSON format
        /// </summary>
        /// <param name="services"></param>
        /// <param name="path"></param>
        /// <param name="userSecret"></param>
        public void Write(IEnumerable<ServiceDefinition> services, string path, byte[] userSecret)
        {
            // Serialise the service definitions to JSON and convert to a byte array
            var json = JsonSerializer.Serialize(services, _options);
            var data = Encoding.UTF8.GetBytes(json);

            // Encrypt the data
            (var encrypted, var salt) = _encryptor.Encrypt(data, userSecret);

            // Combine the salt with the encrypted data
            var combined = Concatenate(salt, encrypted);

            // Write the combination to the data file
            File.WriteAllBytes(path, combined);
        }

        /// <summary>
        /// Concatenate two byte arrays
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static byte[] Concatenate(byte[] first, byte[] second)
        {
            byte[] result = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
            return result;
        }
    }
}