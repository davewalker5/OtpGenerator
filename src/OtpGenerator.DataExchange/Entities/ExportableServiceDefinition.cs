using OtpGenerator.DataExchange.Attributes;
using System;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableServiceDefinition
    {
        public const string CsvRecordPattern = @"^"".*"","".*"","".*""$";

        [Export("Name", 1)]
        public string Name { get; set; }

        [Export("Account", 2)]
        public string Account { get; set;}

        [Export("Secret", 3)]
        public string Secret { get; set; }

        public static ExportableServiceDefinition FromCsv(string record)
        {
            string[] words = record.Split(new string[] { "\",\"" }, StringSplitOptions.None);
            return new ExportableServiceDefinition
            {
                Name = words[0].Replace("\"", "").Trim(),
                Account = words[1].Replace("\"", "").Trim(),
                Secret = words[2].Replace("\"", "").Trim(),
            };
        }
    }
}
