using System.Diagnostics.CodeAnalysis;
using System.Text;
using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.Logic.Cryptography
{
    [ExcludeFromCodeCoverage]
    public class CommandLinePasswordProvider : IUserSecretProvider
    {
        /// <summary>
        /// Prompt for a password with password masking
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public byte[] GetSecret()
        {
            Console.Write("Password: ");

            var password = string.Empty;
            ConsoleKey key;
            do
            {
                // Read and intercept the next key press
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if ((key == ConsoleKey.Backspace) && (password.Length > 0))
                {
                    // Backspace : Delete the last character
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    // Not a control key so add it to the password and echo a "*"
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            }
            while (key != ConsoleKey.Enter);

            Console.WriteLine();

            return Encoding.UTF8.GetBytes(password);
        }
    }
}
