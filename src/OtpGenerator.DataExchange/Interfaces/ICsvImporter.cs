using OtpGenerator.DataExchange.Entities;

namespace OtpGenerator.DataExchange.Interfaces
{
    public interface ICsvImporter<T> where T : class
    {
        event EventHandler<ImportEventArgs<T>> RecordImport;
        void Import(IEnumerable<string> records);
        void Import(string file);
    }
}