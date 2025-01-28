using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ImportEventArgs<T> : EventArgs where T : class
    {
        public long RecordCount { get; set; }
        public T Entity { get; set; }
    }
}
