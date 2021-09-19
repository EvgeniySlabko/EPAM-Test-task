using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with records.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Create new record and adds it to list and dictionaries.
        /// </summary>
        /// <param name="newRecord">Record to add.</param>
        /// <param name="generateNewId">determines whether a new id needs to be generated.</param>
        /// <returns>id of the new record.</returns>
        int CreateRecord(FileCabinetRecord newRecord, bool generateNewId = true);

        /// <summary>
        /// Returns the number of records in the list.
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
        public void Purge();

        /// <summary>
        /// Delete records appropriate conditions.
        /// </summary>
        /// <param name="predicate">Given predicate.</param>
        /// <returns>Deleted records id.</returns>
        public ReadOnlyCollection<int> Delete(Predicate<FileCabinetRecord> predicate);

        /// <summary>
        /// Modify the record matching the condition.
        /// </summary>
        /// <param name="predicate">Given predicate.</param>
        /// /// <param name="action">Action on record.</param>
        /// <returns>Number of changed records.</returns>
        public int Update(Predicate<FileCabinetRecord> predicate, Action<FileCabinetRecord> action);

        /// <summary>
        /// Return parameters of records.
        /// </summary>
        /// <param name="predicate">Given predicate.</param>
        /// <param name="parameters">List of any record parameters.</param>
        /// <returns>Number of changed records.</returns>
        public IEnumerable<List<string>> SelectParameters(Predicate<FileCabinetRecord> predicate, Func<FileCabinetRecord, List<string>> parameters);
    }
}