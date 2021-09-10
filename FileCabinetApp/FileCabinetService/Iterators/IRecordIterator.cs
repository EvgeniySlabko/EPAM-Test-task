using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetRecords iterator.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Get next record.
        /// </summary>
        /// <returns>Next record.</returns>
        public FileCabinetRecord GetNext();

        /// <summary>
        /// Shows if there are more records in the collection.
        /// </summary>
        /// <returns>sign of the presence of records.</returns>
        public bool HasMore();
    }
}
