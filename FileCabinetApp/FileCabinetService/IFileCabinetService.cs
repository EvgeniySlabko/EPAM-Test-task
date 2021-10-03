using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with records.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Insert record.
        /// </summary>
        /// <param name="newRecord">Record to add.</param>
        /// <returns>id of the new record.</returns>
        int Insert(FileCabinetRecord newRecord);

        /// <summary>
        /// Create new record.
        /// </summary>
        /// <param name="newRecord">Record to add.</param>
        /// <returns>id of the new record.</returns>
        int CreateRecord(ValidationRecord newRecord);

        /// <summary>
        /// Returns number of valid records and number of deleted records.
        /// </summary>
        /// <returns>Number of entries in the list.</returns>
        Tuple<int, int> GetStat();

        /// <summary>
        /// Takes a snapshot of the current state of the list of records.
        /// </summary>
        /// <returns>Snapshot of the current list of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restore records.
        /// </summary>
        /// <param name="snapshot">Given snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Purge.
        /// </summary>
        /// <returns>Number of purged records.</returns>
        public int Purge();

        /// <summary>
        /// Delete records appropriate conditions.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <returns>Deleted records id.</returns>
        public ReadOnlyCollection<int> Delete(Query query);

        /// <summary>
        /// Modify the record matching the condition.
        /// </summary>
        /// <param name="query">Query.</param>
        /// /// <param name="action">Action on record.</param>
        /// <returns>Number of changed records.</returns>
        public int Update(Query query, Action<FileCabinetRecord> action);

        /// <summary>
        /// Return parameters of records.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="parameters">List of any record parameters.</param>
        /// <returns>Number of changed records.</returns>
        public IEnumerable<List<string>> SelectParameters(Query query, Func<FileCabinetRecord, List<string>> parameters);
    }
}