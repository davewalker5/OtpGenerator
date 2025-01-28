using System.Text;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Base32;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Generator
{
    public class DummyServiceGenerator : IDummyServiceGenerator
    {
        private const string CharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        private const int MinimumSecretLength = 30;
        private const int MaximumSecretLength = 50;

        private static Random _generator = new();

        /// <summary>
        /// Generate a set of dummy service definitions
        /// </summary>
        /// <param name="name"></param>
        /// <param name="account"></param>
        /// <param name="numberOfDefinitions"></param>
        /// <returns></returns>
        public IList<ServiceDefinition> GenerateDefinitions(string name, string account, int numberOfDefinitions)
        {
            List<ServiceDefinition> definitions = [];

            for (int i = 1; i <= numberOfDefinitions; i++)
            {
                ServiceDefinition definition = new()
                {
                    Id = i,
                    Name = $"{name} #{i}",
                    Account = account,
                    Secret = Base32Encoder.Encode(Encoding.UTF8.GetBytes(RandomSecret()))
                };

                definitions.Add(definition);
            }

            return definitions;
        }
            
            
        /// <summary>
        /// Generate a random secret
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string RandomSecret()
        {
            // Generate a random length for the word, within the specified limits
            var length = _generator.Next(MinimumSecretLength, MaximumSecretLength);

            // Iterate over the number of characters
            StringBuilder builder = new();
            for (int i = 0; i < length; i++)
            {
                // Select a random offset within the character set and append that character
                var offset = (int)_generator.NextInt64(0, CharacterSet.Length);
                builder.Append(CharacterSet[offset]);
            }

            // Get and return the word
            var word =  builder.ToString();
            return word;
        }
    }
}