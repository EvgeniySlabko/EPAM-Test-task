using System;
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
        ReadOnlyCollection<FileCabinetRecord> FindByDate(DateTime dataOfBirthday);

        /// <summary>
        /// Find record by its first name.
        /// </summary>
        /// <param name="firstName">First name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Find record by its last name.
        /// </summary>
        /// <param name="lastName">Last name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>array with records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Returns the number of records in the list.
        /// </summary>
        /// <returns>Number of entries in the list.</returns>
        int GetStat();

        /// <summary>
        /// Takes a snapshot of the current state of the list of records.
        /// </summary>
        /// <returns>Snapshot of the current list of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();
    }
}