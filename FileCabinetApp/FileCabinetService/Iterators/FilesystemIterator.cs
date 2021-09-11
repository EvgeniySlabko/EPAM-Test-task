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
    public class FilesystemIterator : IEnumerator<FileCabinetRecord>
    {
        private readonly Func<int, FileCabonetFilesystemRecord> getNext;
        private readonly Predicate<FileCabinetRecord> comparator;
        private FileCabinetRecord currentRecord;
        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="getter">Get FileCabinetRecord from file using index.</param>
        /// <param name="comparator">Comparator.</param>
        public FilesystemIterator(Func<int, FileCabonetFilesystemRecord> getter, Predicate<FileCabinetRecord> comparator)
        {
            this.comparator = comparator;
            this.getNext = getter;
        }

        /// <inheritdoc/>
        public FileCabinetRecord Current
        {
            get
            {
                if (this.currentRecord is not null)
                {
                    return this.currentRecord;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
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
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            this.GetNext();
            return this.currentRecord is not null;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.index = 0;
            this.GetNext();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        private void GetNext()
        {
            while (true)
            {
                var serviceRecord = this.getNext(this.index++);
                if (serviceRecord is null)
                {
                    this.currentRecord = null;
                    break;
                }

                if ((serviceRecord.ServiceInormation & 4) == 0 && this.comparator(serviceRecord.Record))
                {
                    this.currentRecord = serviceRecord.Record;
                    break;
                }
            }
        }
    }
}
