using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Extension for FileCabonetFilesystemRecord.
    /// </summary>
    public static class FileCabinetFileSystemRecordExtension
    {
        /// <summary>
        /// Shows whether the entry has been deleted.
        /// </summary>
        /// <param name="record">Record.</param>
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
