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
        /// Edits the record by its id.
        /// </summary>
        /// <param name="newRecord">Edited record.</param>
        void Edit(FileCabinetRecord newRecord);

        /// <summary>
        /// Find record by its data of birthday.
        /// </summary>
        /// <param name="dataOfBirthday">Вata of birthday to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        IEnumerable<FileCabinetRecord> FindByDate(DateTime dataOfBirthday);

        /// <summary>
        /// Find record by its first name.
        /// </summary>
        /// <param name="firstName">First name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Find record by its last name.
        /// </summary>
        /// <param name="lastName">Last name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>array with records.</returns>
        IEnumerable<FileCabinetRecord> GetRecords();

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
        /// Remove record from service.
        /// </summary>
        /// <param name="id">The id of the deleted entry.</param>
        public void Remove(int id);

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
    }
}