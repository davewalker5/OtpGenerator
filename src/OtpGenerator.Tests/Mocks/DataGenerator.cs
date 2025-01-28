using System.Text;
using OtpGenerator.Configuration.Entities;
using OtpGenerator.Configuration.Interfaces;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Base32;

namespace OtpGenerator.Tests.Mocks
{
    public static class DataGenerator
    {
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

        private static Random _generator = new Random();

        /// <summary>
        /// Generate a random integer in the specified range
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static int RandomInt(int minimum, int maximum)
            => _generator.Next(minimum, maximum);

        /// <summary>
        /// Generate a random alphanumeric word of the specified length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomWord(int minimumLength, int maximumLength)
        {
            // Generate a random length for the word, within the specified limits
            var length = minimumLength == maximumLength ? minimumLength : RandomInt(minimumLength, maximumLength);

            // Iterate over the number of characters
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                // Select a random offset within the character set and append that character
                var offset = (int)_generator.NextInt64(0, Letters.Length);
                builder.Append(Letters[offset]);
            }

            // Get and return the word
            var word =  builder.ToString();
            return word;
        }

        /// <summary>
        /// Generate some mock application settings
        /// </summary>
        /// <returns></returns>
        public static IOtpGeneratorApplicationSettings MockApplicationSettings()
            => new OtpGeneratorApplicationSettings()
            {
                DatabasePath = "/path/to/the/database/file",
                KeyGenerationParallelism = 4,
                KeyGenerationMemory = 65535,
                KeyGenerationIterations = 3,
                DummyServiceName = "Service",
                DummyServiceAccount = "someone@some.domain.com",
                NumberOfDummyServices = 5
            };

        /// <summary>
        /// Generate a random user secret
        /// </summary>
        /// <returns></returns>
        public static byte[] RandomUserSecret()
            => Encoding.UTF8.GetBytes(DataGenerator.RandomWord(10, 30));

        /// <summary>
        /// Generate a random service name
        /// </summary>
        /// <returns></returns>
        public static string RandomServiceName()
            => RandomWord(10, 30);

        /// <summary>
        /// Generate a random account name
        /// </summary>
        /// <returns></returns>
        public static string RandomAccountName()
            => $"{RandomWord(10, 20)}@{RandomWord(10, 20)}.{RandomWord(3, 3)}";

        /// <summary>
        /// Generate a random service definition secret
        /// </summary>
        /// <returns></returns>
        public static string RandomServiceSecret()
            => Base32Encoder.Encode(Encoding.UTF8.GetBytes(RandomWord(30, 100)));

        /// <summary>
        /// Generate a random service definition
        /// </summary>
        /// <returns></returns>
        public static ServiceDefinition RandomServiceDefinition()
            => new()
            {
                Name = RandomServiceName(),
                Account = RandomAccountName(),
                Secret = RandomServiceSecret()
            };

        /// <summary>
        /// Return the path to a temporary file to act as data storage during tests
        /// </summary>
        /// <returns></returns>
        public static string TemporaryDataFilePath()
            => Path.ChangeExtension(Path.GetTempFileName(), "dat");

        /// <summary>
        /// Return the path to a temporary CSV file to act as data storage during data exchange tests
        /// </summary>
        /// <returns></returns>
        public static string TemporaryExportFilePath()
            => Path.ChangeExtension(Path.GetTempFileName(), "csv");
    }
}