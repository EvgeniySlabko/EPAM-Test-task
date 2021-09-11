using System;
using System.Collections;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetRecordEnumerable.
    /// </summary>
    public class FileCabinetRecordEnumerable : IEnumerable<FileCabinetRecord>
    {
        private readonly IEnumerator<FileCabinetRecord> iterator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordEnumerable"/> class.
        /// </summary>
        /// <param name="iterator">Given iterator.</param>
        public FileCabinetRecordEnumerable(IEnumerator<FileCabinetRecord> iterator)
        {
            this.iterator = iterator;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return this.iterator;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.iterator;
        }
    }
}
