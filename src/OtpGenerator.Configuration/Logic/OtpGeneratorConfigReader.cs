using OtpGenerator.Configuration.Interfaces;
using OtpGenerator.Configuration.Entities;

namespace OtpGenerator.Configuration.Logic
{
    public class OtpGeneratorConfigReader : ConfigReader<OtpGeneratorApplicationSettings>, IConfigReader<OtpGeneratorApplicationSettings>
    {
        /// <summary>
        /// Load and return the application settings from the named JSON-format application settings file
        /// </summary>
        /// <param name="jsonFileName"></param>
        /// <returns></returns>
        public override OtpGeneratorApplicationSettings Read(string jsonFileName)
            => base.Read(jsonFileName);
    }
}
