using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// FilesystemIterator.
    /// </summary>
    public class MemoryIterator : IRecordIterator
    {
        private readonly List<FileCabinetRecord> records;
        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">Record list for iteration.</param>
        public MemoryIterator(List<FileCabinetRecord> records)
        {
            this.records = records;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            return this.records[this.index++];
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return this.index < this.records.Count;
        }
    }
}
