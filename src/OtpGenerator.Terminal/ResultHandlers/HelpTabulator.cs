using OtpGenerator.CommandLine.Entities;
using OtpGenerator.CommandLine.Interfaces;

namespace OtpGenerator.Terminal.ResultHandlers
{
    public class HelpTabulator : TabulatorBase, IHelpGenerator
    {
        /// <summary>
        /// Tabulate a collection of available command line options
        /// </summary>
        /// <param name="options"></param>
        public void Generate(IEnumerable<CommandLineOption> options)
        {
            Initialise();
            SetColumnTitles(["Option", "Short Form", "Description", "Parameters"]);

            foreach (var option in options)
            {
                AddRow([option.Name, option.ShortName, option.Description, option.ParameterDescription], DefaultColour);
            }

            Show();
        }
    }
}
