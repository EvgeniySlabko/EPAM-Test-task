using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// FilesystemIterator.
    /// </summary>
    public class MemoryIterator : IEnumerator<FileCabinetRecord>
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
        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        /// <inheritdoc/>
        public FileCabinetRecord Current
        {
            get
            {
                if (this.index < this.records.Count)
                {
                    return this.records[this.index];
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            this.index++;
            return this.index < this.records.Count;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.index = 0;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
