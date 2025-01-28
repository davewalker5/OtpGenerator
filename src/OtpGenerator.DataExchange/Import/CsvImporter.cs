using OtpGenerator.DataExchange.Interfaces;
using OtpGenerator.DataExchange.Entities;
using OtpGenerator.DataExchange.Exceptions;
using System.Text.RegularExpressions;

namespace OtpGenerator.DataExchange.Import
{
    public abstract class CsvImporter<T> : ICsvImporter<T> where T : class
    {
        private readonly Regex _regex;

        public event EventHandler<ImportEventArgs<T>> RecordImport;

        public CsvImporter(string format)
            => _regex = new Regex(format, RegexOptions.Compiled);

        /// <summary>
        /// Import a collection of CSV format records
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        /// <exception cref="InvalidRecordFormatException"></exception>
        public void Import(IEnumerable<string> records)
        {
            // Perform pre-import preparation
            Prepare();

            // Import is a two-pass process. The first pass performs validation only. The
            // second imports the data. Aside from storage related errors, this should ensure
            // import either works for all records or fails having imported none
            for (int pass = 0; pass < 2; pass++)
            {
                int count = 0;
                foreach (var record in records)
                {
                    count++;
                    if (count > 1)
                    {
                        if (pass == 0)
                        {
                            ValidateRecordFormat(record, count);
                        }
                        else
                        {
                            ImportRecord(record, count);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Import the contents of the CSV file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="factory"></param>
        public void Import(string file)
            => Import(File.ReadAllLines(file));

        /// <summary>
        /// Prepare for import. Override in child classes as needed
        /// </summary>
        /// <returns></returns>
        protected virtual void Prepare()
        {
        }

        /// <summary>
        /// Inflate a record to an object. This should throw an exception if inflation fails
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected abstract T Inflate(string record);

        /// <summary>
        /// Validate an inflated record. This should throw an exception on error
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected abstract void Validate(T entity, int recordCount);

        /// <summary>
        /// Method to store an object in the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract void Add(T entity);

        /// <summary>
        /// Validate a field value, throwing an exception if it's not valid
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <param name="recordCount"></param>
        /// <exception cref="Inv"></exception>
        protected void ValidateField<U>(Predicate<U> predicate, U value, string fieldName, int recordCount)
        {
            if (!predicate(value))
            {
                var message = $"Invalid value for '{fieldName}' at record {recordCount}";
                throw new InvalidFieldValueException(message);
            }
        }

        /// <summary>
        /// Check a record matches the required format and can be inflated to an object
        /// </summary>
        /// <param name="record"></param>
        /// <param name="recordCount"></param>
        /// <exception cref="InvalidRecordFormatException"></exception>
        private void ValidateRecordFormat(string record, int recordCount)
        {
            // Check the line matches the pattern required for successful import
            bool matches = _regex.Matches(record).Any();
            if (!matches)
            {
                string message = $"Invalid record format at line {recordCount}";
                throw new InvalidRecordFormatException(message);
            }

            // Test inflation to an object and validation of the result
            var entity = Inflate(record);
            Validate(entity, recordCount);
        }

        /// <summary>
        /// Inflate a record to an entity and store it in the database
        /// </summary>
        /// <param name="record"></param>
        /// <param name="recordCount"></param>
        private void ImportRecord(string record, int recordCount)
        {
            T entity = Inflate(record);
            Add(entity);
            RecordImport?.Invoke(this, new ImportEventArgs<T> { RecordCount = recordCount - 1, Entity = entity });
        }
    }
}
