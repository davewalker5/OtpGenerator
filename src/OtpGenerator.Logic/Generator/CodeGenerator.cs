using OtpGenerator.Entities.Exceptions;
using OtpGenerator.Entities.Generator;
using OtpGenerator.Logic.Interfaces;
using OtpNet;

namespace OtpGenerator.Logic.Generator
{
    public class CodeGenerator : ICodeGenerator
    {
        /// <summary>
        /// Generate a new timed one-time password for a service
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public TimedOneTimePassword Generate(ServiceDefinition service)
        {
            // Check the secret is valid
            if (string.IsNullOrEmpty(service.Secret))
            {
                var message = $"Service secret cannot be null or empty";
                throw new InvalidServiceSecretException(message);
            }

            // Configure the TOTP generator
            var secret = Base32Encoding.ToBytes(service.Secret);
            var totp = new Totp(secret,
                                mode: OtpHashMode.Sha1,
                                totpSize: 6,
                                step: 30);

            // Generate a new code
            var password = new TimedOneTimePassword
            {
                ServiceId = service.Id,
                ServiceName = service.Name,
                Account = service.Account,
                Code = totp.ComputeTotp(DateTime.UtcNow),
                RemainingSeconds = totp.RemainingSeconds()
            };

            return password;
        }
    }
}