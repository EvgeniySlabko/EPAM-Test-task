using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Records printer.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Print records.
        /// </summary>
        /// <param name="records">Records.</param>
        public void Print(IEnumerable<FileCabinetRecord> records);
    }
}
