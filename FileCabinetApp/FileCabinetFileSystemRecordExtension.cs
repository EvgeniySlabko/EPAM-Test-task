using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Extension for FileCabonetFilesystemRecord.
    /// </summary>
    public static class FileCabinetFileSystemRecordExtension
    {
        /// <summary>
        /// Is deleted.
        /// </summary>
        /// <param name="record">Record</param>
        /// <returns>True if deleted otherwise false.</returns>
        public static bool IsDeleted(this FileCabonetFilesystemRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return (record.ServiceInormation & 4) != 0;
        }
    }
}
