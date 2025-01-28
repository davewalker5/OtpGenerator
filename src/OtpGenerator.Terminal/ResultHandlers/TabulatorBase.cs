using Spectre.Console;

namespace OtpGenerator.Terminal.ResultHandlers
{
    public abstract class TabulatorBase
    {
        public const string DefaultColour = "white";

        public Table Table { get; private set; } = null;

        /// <summary>
        /// Initialise the table
        /// </summary>
        public void Initialise()
            => Table = new Table();

        /// <summary>
        /// Add named columns to the table
        /// </summary>
        /// <param name="columnTitles"></param>
        /// <returns></returns>
        public void SetColumnTitles(IEnumerable<string> columnTitles)
        {
            foreach (var title in columnTitles)
            {
                Table.AddColumn(title);
            }
        }

        /// <summary>
        /// Add a new row to a table given the cell values and a colour to render in
        /// </summary>
        /// <param name="values"></param>
        /// <param name="colour"></param>
        public void AddRow(IEnumerable<string> values, string colour)
            => Table.AddRow(GenerateRowData(values, colour));

        /// <summary>
        /// Insert a row at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="values"></param>
        /// <param name="colour"></param>
        public void InsertRow(int index, IEnumerable<string> values, string colour)
            => Table.InsertRow(index, GenerateRowData(values, colour));

        /// <summary>
        /// Remove the row at the specified index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveRow(int index)
            => Table.RemoveRow(index);

        /// <summary>
        /// Show the table
        /// </summary>
        public void Show()
            => AnsiConsole.Write(Table);

        /// <summary>
        /// Generate the data for a table row using the specified colour
        /// </summary>
        /// <param name="values"></param>
        /// <param name="colour"></param>
        /// <returns></returns>
        private static string[] GenerateRowData(IEnumerable<string> values, string colour)
            => values.Select(x => GetCellData(x, colour)).ToArray();

        /// <summary>
        /// Return the data to populate a single table cell with a given colour
        /// </summary>
        /// <param name="value"></param>
        /// <param name="colour"></param>
        /// <returns></returns>
        private static string GetCellData(string value, string colour)
            => $"[{colour}]{value}[/]";
    }
}