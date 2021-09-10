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
    public class FilesystemIterator : IRecordIterator
    {
        private readonly Func<int, FileCabonetFilesystemRecord> getNext;
        private Predicate<FileCabinetRecord> comparator;
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
            this.GetNext();
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            var outputRecord = this.currentRecord;
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

            return outputRecord;
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return this.currentRecord is not null;
        }
    }
}
